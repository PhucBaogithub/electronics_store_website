using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;
using ElectronicsStore.Models.DTOs;

namespace ElectronicsStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ElectronicsStoreContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(
            ElectronicsStoreContext context,
            UserManager<User> userManager,
            ILogger<ReviewController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<ProductReviewsDTO>> GetProductReviews(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return NotFound("Product not found");
                }

                var reviews = await _context.ProductReviews
                    .Include(r => r.User)
                    .Where(r => r.ProductId == productId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                var reviewDTOs = reviews.Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    ProductId = r.ProductId,
                    ProductName = product.Name,
                    UserId = r.UserId,
                    UserName = r.User.UserName ?? "Unknown",
                    UserFullName = r.User.FullName
                }).ToList();

                var summary = new ReviewSummaryDTO
                {
                    TotalReviews = reviews.Count,
                    AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
                    Rating5Count = reviews.Count(r => r.Rating == 5),
                    Rating4Count = reviews.Count(r => r.Rating == 4),
                    Rating3Count = reviews.Count(r => r.Rating == 3),
                    Rating2Count = reviews.Count(r => r.Rating == 2),
                    Rating1Count = reviews.Count(r => r.Rating == 1)
                };

                bool canReview = false;
                ReviewDTO? userReview = null;

                if (User.Identity?.IsAuthenticated == true)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser != null)
                    {
                        // Check if user has purchased this product
                        var hasPurchased = await _context.OrderItems
                            .Include(oi => oi.Order)
                            .AnyAsync(oi => oi.ProductId == productId && 
                                          oi.Order.UserId == currentUser.Id &&
                                          oi.Order.Status == OrderStatus.Delivered);

                        canReview = hasPurchased;

                        // Check if user has already reviewed this product
                        var existingReview = await _context.ProductReviews
                            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == currentUser.Id);

                        if (existingReview != null)
                        {
                            userReview = new ReviewDTO
                            {
                                Id = existingReview.Id,
                                Rating = existingReview.Rating,
                                Comment = existingReview.Comment,
                                CreatedAt = existingReview.CreatedAt,
                                UpdatedAt = existingReview.UpdatedAt,
                                ProductId = existingReview.ProductId,
                                ProductName = product.Name,
                                UserId = existingReview.UserId,
                                UserName = currentUser.UserName ?? "Unknown",
                                UserFullName = currentUser.FullName
                            };
                        }
                    }
                }

                return Ok(new ProductReviewsDTO
                {
                    Summary = summary,
                    Reviews = reviewDTOs,
                    CanReview = canReview,
                    UserReview = userReview
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product reviews for product {ProductId}", productId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewDTO>> CreateReview([FromBody] CreateReviewDTO createReviewDTO)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var product = await _context.Products.FindAsync(createReviewDTO.ProductId);
                if (product == null)
                {
                    return NotFound("Product not found");
                }

                // Check if user has purchased this product
                var hasPurchased = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .AnyAsync(oi => oi.ProductId == createReviewDTO.ProductId && 
                                  oi.Order.UserId == user.Id &&
                                  oi.Order.Status == OrderStatus.Delivered);

                if (!hasPurchased)
                {
                    return BadRequest("You can only review products you have purchased and received");
                }

                // Check if user has already reviewed this product
                var existingReview = await _context.ProductReviews
                    .FirstOrDefaultAsync(r => r.ProductId == createReviewDTO.ProductId && r.UserId == user.Id);

                if (existingReview != null)
                {
                    return BadRequest("You have already reviewed this product");
                }

                var review = new ProductReview
                {
                    Rating = createReviewDTO.Rating,
                    Comment = createReviewDTO.Comment,
                    ProductId = createReviewDTO.ProductId,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ProductReviews.Add(review);
                await _context.SaveChangesAsync();

                var reviewDTO = new ReviewDTO
                {
                    Id = review.Id,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    UpdatedAt = review.UpdatedAt,
                    ProductId = review.ProductId,
                    ProductName = product.Name,
                    UserId = review.UserId,
                    UserName = user.UserName ?? "Unknown",
                    UserFullName = user.FullName
                };

                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, reviewDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDTO updateReviewDTO)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var review = await _context.ProductReviews
                    .Include(r => r.Product)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (review == null)
                {
                    return NotFound();
                }

                if (review.UserId != user.Id)
                {
                    return Forbid();
                }

                review.Rating = updateReviewDTO.Rating;
                review.Comment = updateReviewDTO.Comment;
                review.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {ReviewId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var review = await _context.ProductReviews.FindAsync(id);
                if (review == null)
                {
                    return NotFound();
                }

                if (review.UserId != user.Id)
                {
                    return Forbid();
                }

                _context.ProductReviews.Remove(review);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDTO>> GetReview(int id)
        {
            try
            {
                var review = await _context.ProductReviews
                    .Include(r => r.User)
                    .Include(r => r.Product)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (review == null)
                {
                    return NotFound();
                }

                var reviewDTO = new ReviewDTO
                {
                    Id = review.Id,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    UpdatedAt = review.UpdatedAt,
                    ProductId = review.ProductId,
                    ProductName = review.Product.Name,
                    UserId = review.UserId,
                    UserName = review.User.UserName ?? "Unknown",
                    UserFullName = review.User.FullName
                };

                return Ok(reviewDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review {ReviewId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetUserReviews()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var reviews = await _context.ProductReviews
                    .Include(r => r.Product)
                    .Where(r => r.UserId == user.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new ReviewDTO
                    {
                        Id = r.Id,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        ProductId = r.ProductId,
                        ProductName = r.Product.Name,
                        UserId = r.UserId,
                        UserName = user.UserName ?? "Unknown",
                        UserFullName = user.FullName
                    })
                    .ToListAsync();

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user reviews");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 