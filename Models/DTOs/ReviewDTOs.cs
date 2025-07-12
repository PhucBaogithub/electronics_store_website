using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models.DTOs
{
    public class CreateReviewDTO
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
        
        [Required]
        public int ProductId { get; set; }
    }
    
    public class UpdateReviewDTO
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
    }
    
    public class ReviewDTO
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
    }
    
    public class ReviewSummaryDTO
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public int Rating5Count { get; set; }
        public int Rating4Count { get; set; }
        public int Rating3Count { get; set; }
        public int Rating2Count { get; set; }
        public int Rating1Count { get; set; }
    }
    
    public class ProductReviewsDTO
    {
        public ReviewSummaryDTO Summary { get; set; } = null!;
        public IEnumerable<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
        public bool CanReview { get; set; } = false;
        public ReviewDTO? UserReview { get; set; }
    }
} 