using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;
using ElectronicsStore.Models.DTOs;

namespace ElectronicsStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ElectronicsStoreContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ElectronicsStoreContext context, ILogger<CategoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .Include(c => c.Products)
                    .OrderBy(c => c.Name)
                    .Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        ImageUrl = c.ImageUrl,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        ProductCount = c.Products.Count(p => p.IsActive)
                    })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound();
                }

                var categoryDto = new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ImageUrl = category.ImageUrl,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    ProductCount = category.Products.Count(p => p.IsActive)
                };

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CreateCategoryDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = new Category
                {
                    Name = model.Name,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                var categoryDto = new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ImageUrl = category.ImageUrl,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    ProductCount = 0
                };

                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDTO model)
        {
            try
            {
                if (id != model.Id)
                {
                    return BadRequest("Category ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                category.Name = model.Name;
                category.Description = model.Description;
                category.ImageUrl = model.ImageUrl;
                category.IsActive = model.IsActive;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound();
                }

                // Check if category has products
                if (category.Products.Any())
                {
                    return BadRequest("Cannot delete category that contains products. Please move products to another category first.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<PagedResultDTO<ProductDTO>>> GetCategoryProducts(int id, [FromQuery] ProductFilterDTO filter)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound("Category not found");
                }

                var query = _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.CategoryId == id && p.IsActive)
                    .AsQueryable();

                // Apply additional filters
                if (!string.IsNullOrEmpty(filter.Search))
                {
                    query = query.Where(p => p.Name.Contains(filter.Search) || 
                                           p.Description.Contains(filter.Search) ||
                                           p.Brand.Contains(filter.Search));
                }

                if (!string.IsNullOrEmpty(filter.Brand))
                {
                    query = query.Where(p => p.Brand.Contains(filter.Brand));
                }

                if (filter.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= filter.MaxPrice.Value);
                }

                if (filter.InStock.HasValue)
                {
                    if (filter.InStock.Value)
                    {
                        query = query.Where(p => p.StockQuantity > 0);
                    }
                    else
                    {
                        query = query.Where(p => p.StockQuantity == 0);
                    }
                }

                // Apply sorting
                switch (filter.SortBy?.ToLower())
                {
                    case "price":
                        query = filter.SortOrder?.ToLower() == "desc" 
                            ? query.OrderByDescending(p => p.Price)
                            : query.OrderBy(p => p.Price);
                        break;
                    case "name":
                        query = filter.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.Name)
                            : query.OrderBy(p => p.Name);
                        break;
                    case "created":
                        query = filter.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.CreatedAt)
                            : query.OrderBy(p => p.CreatedAt);
                        break;
                    default:
                        query = query.OrderBy(p => p.Name);
                        break;
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

                var products = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        DiscountPrice = p.DiscountPrice,
                        StockQuantity = p.StockQuantity,
                        ImageUrl = p.ImageUrl,
                        Brand = p.Brand,
                        Model = p.Model,
                        IsActive = p.IsActive,
                        IsFeatured = p.IsFeatured,
                        CreatedAt = p.CreatedAt,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category.Name,
                        FinalPrice = p.DiscountPrice ?? p.Price,
                        HasDiscount = p.DiscountPrice.HasValue && p.DiscountPrice < p.Price,
                        IsInStock = p.StockQuantity > 0
                    })
                    .ToListAsync();

                return Ok(new PagedResultDTO<ProductDTO>
                {
                    Items = products,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalPages = totalPages,
                    HasPreviousPage = filter.Page > 1,
                    HasNextPage = filter.Page < totalPages
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products for category {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 