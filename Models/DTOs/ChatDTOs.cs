using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models.DTOs
{
    public class SendMessageDTO
    {
        [Required]
        [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
        public string Message { get; set; } = string.Empty;
        
        public string? ConversationId { get; set; }
    }
    
    public class ChatMessageDTO
    {
        public int Id { get; set; }
        public string ConversationId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsFromUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public ChatMessageType MessageType { get; set; }
        public ProductRecommendationDTO? ProductRecommendation { get; set; }
    }
    
    public class ChatResponseDTO
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public ChatMessageDTO? UserMessage { get; set; }
        public ChatMessageDTO? BotResponse { get; set; }
        public string ConversationId { get; set; } = string.Empty;
    }
    
    public class ChatHistoryDTO
    {
        public string ConversationId { get; set; } = string.Empty;
        public List<ChatMessageDTO> Messages { get; set; } = new();
    }
    
    public class ProductRecommendationDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }
        public bool IsInStock { get; set; }
        public string ProductUrl { get; set; } = string.Empty;
    }
    
    public class ChatbotConfigDTO
    {
        public string WelcomeMessage { get; set; } = "Xin chào! Tôi là trợ lý ảo của Electronics Store. Tôi có thể giúp bạn tìm sản phẩm, trả lời câu hỏi về đơn hàng, hoặc hỗ trợ khách hàng. Bạn cần hỗ trợ gì?";
        public List<string> QuickReplies { get; set; } = new()
        {
            "Tìm sản phẩm",
            "Kiểm tra đơn hàng", 
            "Hỗ trợ kỹ thuật",
            "Chính sách bảo hành",
            "Liên hệ nhân viên"
        };
    }
}
