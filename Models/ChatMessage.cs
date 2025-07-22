using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicsStore.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string ConversationId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public bool IsFromUser { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Optional: Link to user if authenticated
        public string? UserId { get; set; }
        
        // Navigation property
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        
        // Message type for different kinds of responses
        public ChatMessageType MessageType { get; set; } = ChatMessageType.Text;
        
        // Optional: Product recommendation data (JSON)
        public string? ProductData { get; set; }
    }
    
    public enum ChatMessageType
    {
        Text,
        ProductRecommendation,
        FAQ,
        EscalateToHuman
    }
}
