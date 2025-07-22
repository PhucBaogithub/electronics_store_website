using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Models;
using ElectronicsStore.Models.DTOs;
using ElectronicsStore.Data;
using ElectronicsStore.Services;
using System.Text.Json;

namespace ElectronicsStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ElectronicsStoreContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ChatController> _logger;
        private readonly IChatbotService _chatbotService;

        public ChatController(
            ElectronicsStoreContext context,
            UserManager<User> userManager,
            ILogger<ChatController> logger,
            IChatbotService chatbotService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _chatbotService = chatbotService;
        }

        [HttpPost("send")]
        public async Task<ActionResult<ChatResponseDTO>> SendMessage([FromBody] SendMessageDTO request)
        {
            try
            {
                // Generate conversation ID if not provided
                var conversationId = request.ConversationId ?? Guid.NewGuid().ToString();
                
                // Get current user if authenticated
                User? currentUser = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    currentUser = await _userManager.GetUserAsync(User);
                }

                // Save user message
                var userMessage = new ChatMessage
                {
                    ConversationId = conversationId,
                    Message = request.Message,
                    IsFromUser = true,
                    UserId = currentUser?.Id,
                    MessageType = ChatMessageType.Text,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ChatMessages.Add(userMessage);
                await _context.SaveChangesAsync();

                // Generate bot response using ChatbotService
                var botResponse = await _chatbotService.GenerateBotResponseAsync(request.Message, conversationId, currentUser);
                
                _context.ChatMessages.Add(botResponse);
                await _context.SaveChangesAsync();

                return Ok(new ChatResponseDTO
                {
                    Success = true,
                    ConversationId = conversationId,
                    UserMessage = MapToDTO(userMessage),
                    BotResponse = MapToDTO(botResponse)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, new ChatResponseDTO
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi xử lý tin nhắn. Vui lòng thử lại."
                });
            }
        }

        [HttpGet("history/{conversationId}")]
        public async Task<ActionResult<ChatHistoryDTO>> GetChatHistory(string conversationId)
        {
            try
            {
                var messages = await _context.ChatMessages
                    .Where(m => m.ConversationId == conversationId)
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();

                return Ok(new ChatHistoryDTO
                {
                    ConversationId = conversationId,
                    Messages = messages.Select(MapToDTO).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat history for conversation {ConversationId}", conversationId);
                return StatusCode(500, "Đã xảy ra lỗi khi tải lịch sử chat.");
            }
        }

        [HttpGet("config")]
        public ActionResult<ChatbotConfigDTO> GetChatbotConfig()
        {
            return Ok(new ChatbotConfigDTO());
        }

        [HttpGet("test")]
        public ActionResult<object> TestChatbot()
        {
            try
            {
                var testInfo = new
                {
                    Success = true,
                    Message = "Chatbot API is working correctly",
                    DatabaseConnected = _context.Database.CanConnect(),
                    ChatMessagesTableExists = _context.ChatMessages != null,
                    CurrentTime = DateTime.UtcNow,
                    SampleResponse = "Xin chào! Tôi là trợ lý ảo của Electronics Store."
                };

                return Ok(testInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in chatbot test endpoint");
                return StatusCode(500, new { Success = false, Error = ex.Message });
            }
        }

        // Phương thức này đã được thay thế bằng ChatbotService

        private ChatMessageDTO MapToDTO(ChatMessage message)
        {
            ProductRecommendationDTO? productRecommendation = null;
            
            if (message.MessageType == ChatMessageType.ProductRecommendation && !string.IsNullOrEmpty(message.ProductData))
            {
                try
                {
                    var products = JsonSerializer.Deserialize<List<ProductRecommendationDTO>>(message.ProductData);
                    productRecommendation = products?.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize product data for message {MessageId}", message.Id);
                }
            }

            return new ChatMessageDTO
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                Message = message.Message,
                IsFromUser = message.IsFromUser,
                CreatedAt = message.CreatedAt,
                MessageType = message.MessageType,
                ProductRecommendation = productRecommendation
            };
        }
    }
}
