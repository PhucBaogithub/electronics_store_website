using ElectronicsStore.Models;
using ElectronicsStore.Models.DTOs;
using ElectronicsStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;

namespace ElectronicsStore.Services
{
    public interface IChatbotService
    {
        Task<ChatMessage> GenerateBotResponseAsync(string userMessage, string conversationId, User? currentUser);
        Task<List<ProductRecommendationDTO>> SearchProductsAsync(string query);
        string GetGreetingResponse();
        string GetHelpResponse();
    }

    public class ChatbotService : IChatbotService
    {
        private readonly ElectronicsStoreContext _context;
        private readonly ILogger<ChatbotService> _logger;

        // Từ khóa và mẫu câu trả lời
        private readonly Dictionary<string, List<string>> _responsePatterns;
        private readonly Dictionary<string, string> _quickResponses;

        public ChatbotService(ElectronicsStoreContext context, ILogger<ChatbotService> logger)
        {
            _context = context;
            _logger = logger;
            _responsePatterns = new Dictionary<string, List<string>>();
            _quickResponses = new Dictionary<string, string>();
            InitializeResponsePatterns();
        }

        private void InitializeResponsePatterns()
        {
            // Initialize response patterns
            _responsePatterns.Add("greeting", new List<string>
            {
                "Xin chào! Tôi là trợ lý ảo của Electronics Store. Tôi có thể giúp bạn tìm sản phẩm, kiểm tra đơn hàng, hoặc trả lời các câu hỏi. Bạn cần hỗ trợ gì?",
                "Chào bạn! Rất vui được hỗ trợ bạn hôm nay. Tôi có thể giúp bạn về sản phẩm điện tử, đơn hàng, hoặc các thông tin khác. Bạn muốn biết gì?",
                "Hello! Tôi là chatbot của Electronics Store. Tôi sẵn sàng hỗ trợ bạn về mọi thứ liên quan đến cửa hàng. Hãy cho tôi biết bạn cần gì nhé!"
            });

            _responsePatterns.Add("product_search", new List<string>
            {
                "Tôi sẽ tìm kiếm sản phẩm phù hợp với yêu cầu của bạn...",
                "Để tìm sản phẩm tốt nhất cho bạn, hãy cho tôi biết thêm về:",
                "Tôi có thể giúp bạn tìm sản phẩm. Bạn đang quan tâm đến loại nào?"
            });

            _responsePatterns.Add("order_inquiry", new List<string>
            {
                "Để kiểm tra đơn hàng, bạn có thể:",
                "Thông tin đơn hàng của bạn:",
                "Tôi sẽ giúp bạn kiểm tra tình trạng đơn hàng"
            });

            _responsePatterns.Add("technical_support", new List<string>
            {
                "Tôi có thể hỗ trợ bạn về các vấn đề kỹ thuật:",
                "Để giải quyết vấn đề kỹ thuật, hãy mô tả chi tiết:",
                "Đội ngũ kỹ thuật của chúng tôi có thể hỗ trợ:"
            });

            _responsePatterns.Add("warranty", new List<string>
            {
                "Chính sách bảo hành của Electronics Store:",
                "Thông tin bảo hành sản phẩm:",
                "Về bảo hành, chúng tôi cam kết:"
            });

            _responsePatterns.Add("price_inquiry", new List<string>
            {
                "Về giá cả sản phẩm:",
                "Thông tin giá và khuyến mãi:",
                "Giá sản phẩm hiện tại:"
            });

            _responsePatterns.Add("shipping", new List<string>
            {
                "Thông tin về giao hàng:",
                "Chính sách vận chuyển của chúng tôi:",
                "Về việc giao hàng:"
            });

            _responsePatterns.Add("payment", new List<string>
            {
                "Các phương thức thanh toán được hỗ trợ:",
                "Về thanh toán:",
                "Chúng tôi chấp nhận các hình thức thanh toán:"
            });

            // Initialize quick responses
            _quickResponses["cảm ơn"] = "Rất vui được hỗ trợ bạn! Nếu có thêm câu hỏi nào khác, đừng ngần ngại hỏi tôi nhé! 😊";
            _quickResponses["tạm biệt"] = "Tạm biệt và cảm ơn bạn đã ghé thăm Electronics Store! Chúc bạn một ngày tốt lành! 👋";
            _quickResponses["ok"] = "Tuyệt vời! Còn gì khác tôi có thể giúp bạn không?";
            _quickResponses["không"] = "Được rồi! Nếu bạn cần hỗ trợ gì khác, hãy cho tôi biết nhé!";
            _quickResponses["có"] = "Tốt! Hãy cho tôi biết chi tiết hơn để tôi có thể hỗ trợ bạn tốt nhất.";
            _quickResponses["help"] = "Tôi có thể giúp bạn về: 🔍 Tìm sản phẩm, 📦 Kiểm tra đơn hàng, 🛠️ Hỗ trợ kỹ thuật, 🛡️ Bảo hành, 💰 Giá cả, 🚚 Giao hàng, 💳 Thanh toán";
        }

        public async Task<ChatMessage> GenerateBotResponseAsync(string userMessage, string conversationId, User? currentUser)
        {
            var message = NormalizeVietnameseText(userMessage.ToLower().Trim());
            var responseText = "";
            var messageType = ChatMessageType.Text;
            string? productData = null;

            try
            {
                // Kiểm tra quick responses trước
                var quickResponse = GetQuickResponse(message);
                if (!string.IsNullOrEmpty(quickResponse))
                {
                    responseText = quickResponse;
                }
                // Tìm kiếm sản phẩm
                else if (IsProductSearchQuery(message))
                {
                    var products = await SearchProductsAsync(message);
                    if (products.Any())
                    {
                        responseText = FormatProductSearchResponse(products, message);
                        messageType = ChatMessageType.ProductRecommendation;
                        productData = JsonSerializer.Serialize(products);
                    }
                    else
                    {
                        responseText = "Xin lỗi, tôi không tìm thấy sản phẩm nào phù hợp với yêu cầu của bạn. Bạn có thể thử:\n" +
                                     "• Mô tả chi tiết hơn về sản phẩm\n" +
                                     "• Sử dụng từ khóa khác\n" +
                                     "• Liên hệ nhân viên tư vấn để được hỗ trợ trực tiếp";
                    }
                }
                // Kiểm tra đơn hàng
                else if (IsOrderInquiry(message))
                {
                    responseText = GetOrderInquiryResponse(currentUser);
                }
                // Hỗ trợ kỹ thuật
                else if (IsTechnicalSupport(message))
                {
                    responseText = GetTechnicalSupportResponse();
                }
                // Bảo hành
                else if (IsWarrantyInquiry(message))
                {
                    responseText = GetWarrantyResponse();
                }
                // Giá cả
                else if (IsPriceInquiry(message))
                {
                    responseText = GetPriceInquiryResponse();
                }
                // Giao hàng
                else if (IsShippingInquiry(message))
                {
                    responseText = GetShippingResponse();
                }
                // Thanh toán
                else if (IsPaymentInquiry(message))
                {
                    responseText = GetPaymentResponse();
                }
                // Chào hỏi
                else if (IsGreeting(message))
                {
                    responseText = GetRandomResponse("greeting");
                }
                // Yêu cầu hỗ trợ nhân viên
                else if (IsHumanSupportRequest(message))
                {
                    responseText = GetHumanSupportResponse();
                    messageType = ChatMessageType.EscalateToHuman;
                }
                // Phản hồi mặc định
                else
                {
                    responseText = GetDefaultResponse(message);
                }

                return new ChatMessage
                {
                    ConversationId = conversationId,
                    Message = responseText,
                    IsFromUser = false,
                    MessageType = messageType,
                    ProductData = productData,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating bot response for message: {Message}", userMessage);
                return new ChatMessage
                {
                    ConversationId = conversationId,
                    Message = "Xin lỗi, tôi gặp một chút vấn đề kỹ thuật. Vui lòng thử lại sau hoặc liên hệ nhân viên hỗ trợ.",
                    IsFromUser = false,
                    MessageType = ChatMessageType.Text,
                    CreatedAt = DateTime.UtcNow
                };
            }
        }

        public async Task<List<ProductRecommendationDTO>> SearchProductsAsync(string query)
        {
            var normalizedQuery = NormalizeVietnameseText(query);
            var keywords = ExtractKeywords(normalizedQuery);

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && keywords.Any(k =>
                    p.Name.ToLower().Contains(k) ||
                    p.Description.ToLower().Contains(k) ||
                    p.Brand.ToLower().Contains(k) ||
                    p.Category!.Name.ToLower().Contains(k)))
                .OrderByDescending(p => p.IsFeatured)
                .ThenBy(p => (double)p.Price)
                .Take(5)
                .Select(p => new ProductRecommendationDTO
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Brand = p.Brand,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    ImageUrl = p.ImageUrl,
                    CategoryName = p.Category!.Name,
                    IsInStock = p.StockQuantity > 0,
                    ProductUrl = $"/Home/ProductDetail/{p.Id}"
                })
                .ToListAsync();

            return products;
        }

        public string GetGreetingResponse()
        {
            return GetRandomResponse("greeting");
        }

        public string GetHelpResponse()
        {
            return _quickResponses["help"];
        }

        private string FormatProductSearchResponse(List<ProductRecommendationDTO> products, string query)
        {
            var response = new StringBuilder();
            response.AppendLine($"Tôi tìm thấy {products.Count} sản phẩm phù hợp với \"{query}\":");
            response.AppendLine();

            for (int i = 0; i < products.Count; i++)
            {
                var product = products[i];
                response.AppendLine($"{i + 1}. {product.Name}");
                response.AppendLine($"   Thương hiệu: {product.Brand}");
                response.AppendLine($"   Danh mục: {product.CategoryName}");

                if (product.DiscountPrice.HasValue)
                {
                    response.AppendLine($"   Giá: {product.Price:N0}₫ -> {product.DiscountPrice:N0}₫ (Giảm giá!)");
                }
                else
                {
                    response.AppendLine($"   Giá: {product.Price:N0}₫");
                }

                response.AppendLine($"   Tình trạng: {(product.IsInStock ? "Còn hàng" : "Hết hàng")}");

                if (i < products.Count - 1)
                {
                    response.AppendLine();
                }
            }

            response.AppendLine();
            response.AppendLine("Bạn có thể:");
            response.AppendLine("• Nhấn vào tên sản phẩm để xem chi tiết");
            response.AppendLine("• Hỏi thêm về sản phẩm cụ thể");
            response.AppendLine("• Yêu cầu so sánh giữa các sản phẩm");
            response.AppendLine("• Liên hệ nhân viên để được tư vấn thêm");

            return response.ToString();
        }

        // Các phương thức helper
        private string NormalizeVietnameseText(string text)
        {
            // Loại bỏ dấu câu và ký tự đặc biệt
            text = Regex.Replace(text, @"[^\w\s]", " ");
            // Loại bỏ khoảng trắng thừa
            text = Regex.Replace(text, @"\s+", " ");
            return text.Trim();
        }

        private List<string> ExtractKeywords(string text)
        {
            var stopWords = new HashSet<string> { "tôi", "bạn", "của", "và", "có", "là", "trong", "với", "để", "một", "các", "này", "đó", "cho", "về", "từ", "như", "sẽ", "được", "hay", "hoặc", "nào", "gì", "ai", "đâu", "khi", "nếu", "thì" };
            
            return text.Split(' ')
                      .Where(word => word.Length > 2 && !stopWords.Contains(word))
                      .Distinct()
                      .ToList();
        }

        private string GetQuickResponse(string message)
        {
            foreach (var kvp in _quickResponses)
            {
                if (message.Contains(kvp.Key))
                {
                    return kvp.Value;
                }
            }
            return string.Empty;
        }

        private string GetRandomResponse(string category)
        {
            if (_responsePatterns.ContainsKey(category))
            {
                var responses = _responsePatterns[category];
                var random = new Random();
                return responses[random.Next(responses.Count)];
            }
            return "Tôi hiểu yêu cầu của bạn và sẽ cố gắng hỗ trợ tốt nhất.";
        }

        private bool IsProductSearchQuery(string message)
        {
            var productKeywords = new[] { "tìm", "mua", "sản phẩm", "laptop", "điện thoại", "máy tính", "tai nghe", "chuột", "bàn phím", "màn hình", "camera", "loa", "tivi", "tủ lạnh", "máy giặt", "điều hòa", "smartphone", "tablet", "iphone", "samsung", "dell", "hp", "asus", "acer", "sony", "lg", "xiaomi", "oppo", "vivo", "realme" };
            return productKeywords.Any(keyword => message.Contains(keyword));
        }

        private bool IsOrderInquiry(string message)
        {
            var orderKeywords = new[] { "đơn hàng", "order", "kiểm tra", "tình trạng", "giao hàng", "vận chuyển", "ship" };
            return orderKeywords.Any(keyword => message.Contains(keyword));
        }

        private bool IsTechnicalSupport(string message)
        {
            var techKeywords = new[] { "hỗ trợ", "kỹ thuật", "lỗi", "sửa chữa", "không hoạt động", "bị hỏng", "cài đặt", "hướng dẫn", "cách sử dụng" };
            return techKeywords.Any(keyword => message.Contains(keyword));
        }

        private bool IsWarrantyInquiry(string message)
        {
            var warrantyKeywords = new[] { "bảo hành", "warranty", "đổi trả", "bảo hiểm" };
            return warrantyKeywords.Any(keyword => message.Contains(keyword));
        }

        private bool IsPriceInquiry(string message)
        {
            var priceKeywords = new[] { "giá", "price", "bao nhiêu", "tiền", "cost", "khuyến mãi", "giảm giá", "sale" };
            return priceKeywords.Any(keyword => message.Contains(keyword));
        }

        private bool IsShippingInquiry(string message)
        {
            var shippingKeywords = new[] { "giao hàng", "vận chuyển", "ship", "delivery", "phí ship", "miễn phí" };
            return shippingKeywords.Any(keyword => message.Contains(keyword));
        }

        private bool IsPaymentInquiry(string message)
        {
            var paymentKeywords = new[] { "thanh toán", "payment", "trả tiền", "thẻ", "chuyển khoản", "cod", "visa", "mastercard", "momo", "zalopay" };
            return paymentKeywords.Any(keyword => message.Contains(keyword));
        }

        private bool IsGreeting(string message)
        {
            var greetingKeywords = new[] { "xin chào", "chào", "hello", "hi", "hey", "good morning", "good afternoon", "good evening" };
            return greetingKeywords.Any(keyword => message.Contains(keyword));
        }

        private bool IsHumanSupportRequest(string message)
        {
            var humanKeywords = new[] { "nhân viên", "tư vấn", "gọi", "liên hệ", "support", "admin", "operator", "người thật" };
            return humanKeywords.Any(keyword => message.Contains(keyword));
        }

        private string GetOrderInquiryResponse(User? currentUser)
        {
            if (currentUser != null)
            {
                return GetRandomResponse("order_inquiry") + "\n" +
                       "• Truy cập 'Đơn hàng của tôi' trong menu tài khoản\n" +
                       "• Kiểm tra email xác nhận đơn hàng\n" +
                       "• Gọi hotline: (028) 1234-5678 để được hỗ trợ trực tiếp\n\n" +
                       "Bạn có cần hỗ trợ gì khác về đơn hàng không?";
            }
            else
            {
                return "Để kiểm tra đơn hàng, bạn cần:\n" +
                       "• Đăng nhập vào tài khoản của bạn\n" +
                       "• Hoặc cung cấp mã đơn hàng và email\n" +
                       "• Liên hệ hotline: (028) 1234-5678\n\n" +
                       "Tôi có thể hướng dẫn bạn đăng nhập không?";
            }
        }

        private string GetTechnicalSupportResponse()
        {
            return GetRandomResponse("technical_support") + "\n" +
                   "🛠️ **Hỗ trợ kỹ thuật 24/7:**\n" +
                   "• Hướng dẫn cài đặt và sử dụng sản phẩm\n" +
                   "• Khắc phục sự cố phần mềm\n" +
                   "• Tư vấn nâng cấp và bảo trì\n" +
                   "• Hỗ trợ từ xa qua TeamViewer\n\n" +
                   "📞 **Liên hệ:**\n" +
                   "• Hotline: (028) 1234-5678\n" +
                   "• Email: tech@electronicsstore.com\n" +
                   "• Chat trực tiếp: Đang kết nối...\n\n" +
                   "Bạn gặp vấn đề gì cụ thể với sản phẩm?";
        }

        private string GetWarrantyResponse()
        {
            return GetRandomResponse("warranty") + "\n" +
                   "🛡️ **Chính sách bảo hành toàn diện:**\n" +
                   "• Bảo hành chính hãng: 12-36 tháng\n" +
                   "• Đổi trả miễn phí trong 7 ngày đầu\n" +
                   "• Sửa chữa miễn phí trong thời gian bảo hành\n" +
                   "• Hỗ trợ kỹ thuật suốt đời sản phẩm\n" +
                   "• Bảo hành tại 200+ trung tâm ủy quyền\n\n" +
                   "📋 **Điều kiện bảo hành:**\n" +
                   "• Sản phẩm còn nguyên tem bảo hành\n" +
                   "• Có hóa đơn mua hàng\n" +
                   "• Không có dấu hiệu va đập, ngấm nước\n\n" +
                   "Bạn cần kiểm tra bảo hành cho sản phẩm nào?";
        }

        private string GetPriceInquiryResponse()
        {
            return GetRandomResponse("price_inquiry") + "\n" +
                   "💰 **Chính sách giá tốt nhất:**\n" +
                   "• Cam kết giá tốt nhất thị trường\n" +
                   "• Hoàn tiền nếu tìm thấy giá rẻ hơn\n" +
                   "• Khuyến mãi đặc biệt mỗi tuần\n" +
                   "• Giảm giá thêm cho khách hàng thân thiết\n\n" +
                   "🎁 **Ưu đãi hiện tại:**\n" +
                   "• Giảm 10% cho đơn hàng trên 5 triệu\n" +
                   "• Tặng phụ kiện khi mua laptop\n" +
                   "• Trả góp 0% lãi suất\n\n" +
                   "Bạn quan tâm đến sản phẩm nào để tôi báo giá cụ thể?";
        }

        private string GetShippingResponse()
        {
            return GetRandomResponse("shipping") + "\n" +
                   "🚚 **Dịch vụ giao hàng nhanh chóng:**\n" +
                   "• Giao hàng nhanh trong 2-4 giờ (nội thành)\n" +
                   "• Giao hàng toàn quốc 1-3 ngày\n" +
                   "• Miễn phí ship cho đơn hàng trên 500k\n" +
                   "• Giao hàng tận nơi, kiểm tra trước khi nhận\n\n" +
                   "📦 **Các hình thức giao hàng:**\n" +
                   "• Giao hàng tiêu chuẩn: 15,000đ\n" +
                   "• Giao hàng nhanh: 25,000đ\n" +
                   "• Giao hàng siêu tốc: 35,000đ\n\n" +
                   "Bạn muốn đặt hàng với hình thức giao hàng nào?";
        }

        private string GetPaymentResponse()
        {
            return GetRandomResponse("payment") + "\n" +
                   "💳 **Đa dạng phương thức thanh toán:**\n" +
                   "• Thanh toán khi nhận hàng (COD)\n" +
                   "• Chuyển khoản ngân hàng\n" +
                   "• Thẻ tín dụng/ghi nợ (Visa, Mastercard)\n" +
                   "• Ví điện tử (MoMo, ZaloPay, VNPay)\n" +
                   "• Trả góp qua thẻ tín dụng\n\n" +
                   "🏦 **Thông tin chuyển khoản:**\n" +
                   "• Ngân hàng: Vietcombank\n" +
                   "• STK: 1234567890\n" +
                   "• Chủ TK: Electronics Store\n\n" +
                   "Bạn muốn thanh toán bằng hình thức nào?";
        }

        private string GetHumanSupportResponse()
        {
            return "🙋‍♂️ **Kết nối với nhân viên tư vấn:**\n\n" +
                   "Tôi sẽ chuyển bạn đến nhân viên tư vấn chuyên nghiệp:\n\n" +
                   "📞 **Liên hệ trực tiếp:**\n" +
                   "• Hotline: (028) 1234-5678\n" +
                   "• Email: support@electronicsstore.com\n" +
                   "• Zalo: 0901234567\n\n" +
                   "🕐 **Thời gian hỗ trợ:**\n" +
                   "• Thứ 2 - Thứ 6: 8:00 - 17:00\n" +
                   "• Thứ 7 - Chủ nhật: 8:00 - 12:00\n\n" +
                   "💬 **Chat trực tiếp:** Đang kết nối với nhân viên...\n\n" +
                   "Nhân viên sẽ liên hệ với bạn trong vòng 2-3 phút!";
        }

        private string GetDefaultResponse(string message)
        {
            var responses = new[]
            {
                "Tôi hiểu bạn đang cần hỗ trợ. Để tôi có thể giúp bạn tốt nhất, bạn có thể:\n" +
                "🔍 Tìm kiếm sản phẩm cụ thể\n" +
                "📦 Hỏi về đơn hàng\n" +
                "🛠️ Yêu cầu hỗ trợ kỹ thuật\n" +
                "💰 Hỏi về giá cả và khuyến mãi\n" +
                "🚚 Thông tin giao hàng\n" +
                "👨‍💼 Kết nối với nhân viên tư vấn",

                "Cảm ơn bạn đã liên hệ! Tôi có thể hỗ trợ bạn về:\n" +
                "• Tư vấn sản phẩm điện tử\n" +
                "• Kiểm tra đơn hàng và giao hàng\n" +
                "• Chính sách bảo hành và đổi trả\n" +
                "• Hướng dẫn sử dụng sản phẩm\n\n" +
                "Hãy mô tả cụ thể hơn về điều bạn cần để tôi hỗ trợ tốt nhất nhé!",

                "Tôi muốn giúp bạn nhưng chưa hiểu rõ yêu cầu. Bạn có thể:\n" +
                "• Sử dụng từ khóa đơn giản hơn\n" +
                "• Mô tả chi tiết vấn đề\n" +
                "• Chọn một trong các chủ đề: Sản phẩm, Đơn hàng, Hỗ trợ, Bảo hành\n" +
                "• Hoặc gõ 'help' để xem danh sách hỗ trợ"
            };

            var random = new Random();
            return responses[random.Next(responses.Length)];
        }
    }
}
