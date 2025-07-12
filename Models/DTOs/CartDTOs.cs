using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models.DTOs
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsInStock { get; set; }
        public int StockQuantity { get; set; }
        public DateTime DateAdded { get; set; }
    }
    
    public class AddToCartDTO
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
    
    public class UpdateCartItemDTO
    {
        [Required]
        public int CartItemId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
    
    public class CartSummaryDTO
    {
        public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
    }
    
    public class OrderDTO
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }
    
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
    
    public class CreateOrderDTO
    {
        [Required]
        [StringLength(200)]
        public string ShippingFirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string ShippingLastName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string ShippingCity { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string ShippingPostalCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string ShippingCountry { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string ShippingPhone { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        public string PaymentMethod { get; set; } = "COD"; // Cash on Delivery by default
    }
    
    public class UpdateOrderStatusDTO
    {
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public string Status { get; set; } = string.Empty;
        
        public string Notes { get; set; } = string.Empty;
    }
    
    public class OrderFilterDTO
    {
        public string? OrderNumber { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? UserId { get; set; }
        public string? SortBy { get; set; } = "orderDate";
        public string? SortOrder { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
} 