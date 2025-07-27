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
    public class ProductsController : ControllerBase
    {
        private readonly ElectronicsStoreContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ElectronicsStoreContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDTO<ProductDTO>>> GetProducts([FromQuery] ProductFilterDTO filter)
        {
            try
            {
                var query = _context.Products.Include(p => p.Category).AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(filter.Search))
                {
                    query = query.Where(p => p.Name.Contains(filter.Search) || 
                                           p.Description.Contains(filter.Search) ||
                                           p.Brand.Contains(filter.Search));
                }

                if (filter.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
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

                if (filter.Featured.HasValue)
                {
                    query = query.Where(p => p.IsFeatured == filter.Featured.Value);
                }

                query = query.Where(p => p.IsActive);

                // Apply sorting - convert to double to work with SQLite
                switch (filter.SortBy?.ToLower())
                {
                    case "price":
                        query = filter.SortOrder?.ToLower() == "desc" 
                            ? query.OrderByDescending(p => (double)p.Price)
                            : query.OrderBy(p => (double)p.Price);
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
                _logger.LogError(ex, "Error retrieving products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound();
                }

                var productDto = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                    StockQuantity = product.StockQuantity,
                    ImageUrl = product.ImageUrl,
                    Brand = product.Brand,
                    Model = product.Model,
                    IsActive = product.IsActive,
                    IsFeatured = product.IsFeatured,
                    CreatedAt = product.CreatedAt,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name,
                    FinalPrice = product.DiscountPrice ?? product.Price,
                    HasDiscount = product.DiscountPrice.HasValue && product.DiscountPrice < product.Price,
                    IsInStock = product.StockQuantity > 0
                };

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> SearchProducts([FromQuery] string query, [FromQuery] int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Search query cannot be empty");
                }

                var products = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsActive && (
                        p.Name.Contains(query) ||
                        p.Description.Contains(query) ||
                        p.Brand.Contains(query) ||
                        p.Category.Name.Contains(query)
                    ))
                    .Take(limit)
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        DiscountPrice = p.DiscountPrice,
                        ImageUrl = p.ImageUrl,
                        Brand = p.Brand,
                        CategoryName = p.Category.Name,
                        FinalPrice = p.DiscountPrice ?? p.Price,
                        HasDiscount = p.DiscountPrice.HasValue && p.DiscountPrice < p.Price,
                        IsInStock = p.StockQuantity > 0
                    })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with query: {Query}", query);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetFeaturedProducts()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsFeatured && p.IsActive)
                    .Take(8)
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

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving featured products");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get related products based on the same category as the specified product
        /// </summary>
        /// <param name="id">Product ID to find related products for</param>
        /// <param name="count">Number of related products to return (default: 6)</param>
        /// <returns>List of related products</returns>
        [HttpGet("{id}/related")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetRelatedProducts(int id, [FromQuery] int count = 6)
        {
            try
            {
                // First, get the current product to find its category
                var currentProduct = await _context.Products
                    .Where(p => p.Id == id && p.IsActive)
                    .FirstOrDefaultAsync();

                if (currentProduct == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                // Get related products from the same category, excluding the current product
                var relatedProducts = await _context.Products
                    .Where(p => p.IsActive &&
                               p.CategoryId == currentProduct.CategoryId &&
                               p.Id != id) // Exclude current product
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.IsFeatured) // Prioritize featured products
                    .ThenByDescending(p => p.CreatedAt) // Then by newest
                    .Take(count)
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
                        CategoryName = p.Category != null ? p.Category.Name : "",
                        FinalPrice = p.DiscountPrice ?? p.Price,
                        HasDiscount = p.DiscountPrice.HasValue && p.DiscountPrice < p.Price,
                        IsInStock = p.StockQuantity > 0
                    })
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} related products for product ID {ProductId} in category {CategoryId}",
                    relatedProducts.Count, id, currentProduct.CategoryId);

                return Ok(relatedProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving related products for product ID {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] CreateProductDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    DiscountPrice = model.DiscountPrice,
                    StockQuantity = model.StockQuantity,
                    ImageUrl = model.ImageUrl,
                    Brand = model.Brand,
                    Model = model.Model,
                    IsActive = model.IsActive,
                    IsFeatured = model.IsFeatured,
                    CategoryId = model.CategoryId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                var category = await _context.Categories.FindAsync(product.CategoryId);

                var productDto = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                    StockQuantity = product.StockQuantity,
                    ImageUrl = product.ImageUrl,
                    Brand = product.Brand,
                    Model = product.Model,
                    IsActive = product.IsActive,
                    IsFeatured = product.IsFeatured,
                    CreatedAt = product.CreatedAt,
                    CategoryId = product.CategoryId,
                    CategoryName = category?.Name ?? "",
                    FinalPrice = product.DiscountPrice ?? product.Price,
                    HasDiscount = product.DiscountPrice.HasValue && product.DiscountPrice < product.Price,
                    IsInStock = product.StockQuantity > 0
                };

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO model)
        {
            try
            {
                if (id != model.Id)
                {
                    return BadRequest("Product ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.DiscountPrice = model.DiscountPrice;
                product.StockQuantity = model.StockQuantity;
                product.ImageUrl = model.ImageUrl;
                product.Brand = model.Brand;
                product.Model = model.Model;
                product.IsActive = model.IsActive;
                product.IsFeatured = model.IsFeatured;
                product.CategoryId = model.CategoryId;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 