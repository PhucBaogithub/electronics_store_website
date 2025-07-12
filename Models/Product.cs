using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicsStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public int StockQuantity { get; set; }
        
        public string? ImageUrl { get; set; }
        
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public bool IsFeatured { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign key
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }
        
        // Navigation properties  
        public virtual Category? Category { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        
        // Computed properties
        public decimal FinalPrice => DiscountPrice ?? Price;
        public bool HasDiscount => DiscountPrice.HasValue && DiscountPrice < Price;
        public bool IsInStock => StockQuantity > 0;
    }
} 