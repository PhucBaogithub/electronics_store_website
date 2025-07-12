using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicsStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        public DateTime DateAdded { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Foreign keys
        [Required]
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        
        // Computed properties
        [NotMapped]
        public decimal UnitPrice => Price;
        
        [NotMapped]
        public decimal TotalPrice => Price * Quantity;
    }
} 