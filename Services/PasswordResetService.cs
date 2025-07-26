using ElectronicsStore.Data;
using ElectronicsStore.Models;
using ElectronicsStore.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ElectronicsStore.Services
{
    public interface IPasswordResetService
    {
        Task<PasswordResetResponseDTO> SendResetCodeAsync(string email, string ipAddress, string userAgent);
        Task<PasswordResetResponseDTO> VerifyResetCodeAsync(string email, string resetCode);
        Task<PasswordResetResponseDTO> ResetPasswordAsync(string email, string resetCode, string newPassword, string ipAddress, string userAgent);
        Task<bool> CanRequestResetAsync(string email);
        Task CleanupExpiredTokensAsync();
    }

    public class PasswordResetService : IPasswordResetService
    {
        private readonly ElectronicsStoreContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<PasswordResetService> _logger;
        private readonly IConfiguration _configuration;

        // Rate limiting: 1 request per 5 minutes per email
        private const int RATE_LIMIT_MINUTES = 5;
        private const int TOKEN_EXPIRY_MINUTES = 15;
        private const int MAX_ATTEMPTS_PER_DAY = 5;

        public PasswordResetService(
            ElectronicsStoreContext context,
            UserManager<User> userManager,
            IEmailService emailService,
            ILogger<PasswordResetService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<PasswordResetResponseDTO> SendResetCodeAsync(string email, string ipAddress, string userAgent)
        {
            try
            {
                // Check if user exists
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Don't reveal that user doesn't exist for security
                    return new PasswordResetResponseDTO
                    {
                        Success = true,
                        Message = "Nếu email tồn tại trong hệ thống, mã xác nhận đã được gửi."
                    };
                }

                // Check rate limiting
                if (!await CanRequestResetAsync(email))
                {
                    var lastRequest = await _context.PasswordResetTokens
                        .Where(t => t.Email == email)
                        .OrderByDescending(t => t.CreatedAt)
                        .FirstOrDefaultAsync();

                    var nextAllowedTime = lastRequest?.CreatedAt.AddMinutes(RATE_LIMIT_MINUTES);
                    var waitTime = nextAllowedTime - DateTime.UtcNow;

                    return new PasswordResetResponseDTO
                    {
                        Success = false,
                        Message = $"Vui lòng đợi {waitTime?.Minutes} phút {waitTime?.Seconds} giây trước khi gửi lại.",
                        RetryAfter = waitTime
                    };
                }

                // Check daily limit
                var todayRequests = await _context.PasswordResetTokens
                    .CountAsync(t => t.Email == email && t.CreatedAt >= DateTime.UtcNow.Date);

                if (todayRequests >= MAX_ATTEMPTS_PER_DAY)
                {
                    return new PasswordResetResponseDTO
                    {
                        Success = false,
                        Message = "Bạn đã vượt quá số lần yêu cầu đặt lại mật khẩu trong ngày. Vui lòng thử lại vào ngày mai.",
                        RemainingAttempts = 0
                    };
                }

                // Generate 6-digit reset code
                var resetCode = GenerateResetCode();
                var tokenHash = HashResetCode(resetCode);

                // Create reset token
                var resetToken = new PasswordResetToken
                {
                    UserId = user.Id,
                    TokenHash = tokenHash,
                    Email = email,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(TOKEN_EXPIRY_MINUTES),
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                _context.PasswordResetTokens.Add(resetToken);
                await _context.SaveChangesAsync();

                // Send email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(
                    email, 
                    resetCode, 
                    user.FirstName ?? user.UserName ?? "Khách hàng"
                );

                if (!emailSent)
                {
                    _logger.LogError("Failed to send password reset email to {Email}", email);
                    return new PasswordResetResponseDTO
                    {
                        Success = false,
                        Message = "Có lỗi xảy ra khi gửi email. Vui lòng thử lại sau."
                    };
                }

                _logger.LogInformation("Password reset code sent to {Email} from IP {IP}", email, ipAddress);

                return new PasswordResetResponseDTO
                {
                    Success = true,
                    Message = "Mã xác nhận đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư.",
                    ExpiresAt = resetToken.ExpiresAt,
                    RemainingAttempts = MAX_ATTEMPTS_PER_DAY - todayRequests - 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reset code to {Email}", email);
                return new PasswordResetResponseDTO
                {
                    Success = false,
                    Message = "Có lỗi xảy ra. Vui lòng thử lại sau."
                };
            }
        }

        public async Task<PasswordResetResponseDTO> VerifyResetCodeAsync(string email, string resetCode)
        {
            try
            {
                var tokenHash = HashResetCode(resetCode);
                
                var resetToken = await _context.PasswordResetTokens
                    .Where(t => t.Email == email && t.TokenHash == tokenHash && !t.IsUsed)
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefaultAsync();

                if (resetToken == null)
                {
                    return new PasswordResetResponseDTO
                    {
                        Success = false,
                        Message = "Mã xác nhận không hợp lệ."
                    };
                }

                if (resetToken.IsExpired)
                {
                    return new PasswordResetResponseDTO
                    {
                        Success = false,
                        Message = "Mã xác nhận đã hết hạn. Vui lòng yêu cầu mã mới."
                    };
                }

                return new PasswordResetResponseDTO
                {
                    Success = true,
                    Message = "Mã xác nhận hợp lệ."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying reset code for {Email}", email);
                return new PasswordResetResponseDTO
                {
                    Success = false,
                    Message = "Có lỗi xảy ra. Vui lòng thử lại sau."
                };
            }
        }

        public async Task<PasswordResetResponseDTO> ResetPasswordAsync(string email, string resetCode, string newPassword, string ipAddress, string userAgent)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new PasswordResetResponseDTO
                    {
                        Success = false,
                        Message = "Người dùng không tồn tại."
                    };
                }

                var tokenHash = HashResetCode(resetCode);
                
                var resetToken = await _context.PasswordResetTokens
                    .Where(t => t.Email == email && t.TokenHash == tokenHash && !t.IsUsed)
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefaultAsync();

                if (resetToken == null || resetToken.IsExpired)
                {
                    return new PasswordResetResponseDTO
                    {
                        Success = false,
                        Message = "Mã xác nhận không hợp lệ hoặc đã hết hạn."
                    };
                }

                // Reset password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (!result.Succeeded)
                {
                    return new PasswordResetResponseDTO
                    {
                        Success = false,
                        Message = "Không thể đặt lại mật khẩu.",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                // Mark token as used
                resetToken.IsUsed = true;
                resetToken.UsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Send success email
                await _emailService.SendPasswordResetSuccessEmailAsync(
                    email, 
                    user.FirstName ?? user.UserName ?? "Khách hàng"
                );

                _logger.LogInformation("Password reset successful for {Email} from IP {IP}", email, ipAddress);

                return new PasswordResetResponseDTO
                {
                    Success = true,
                    Message = "Mật khẩu đã được đặt lại thành công. Bạn có thể đăng nhập bằng mật khẩu mới."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for {Email}", email);
                return new PasswordResetResponseDTO
                {
                    Success = false,
                    Message = "Có lỗi xảy ra. Vui lòng thử lại sau."
                };
            }
        }

        public async Task<bool> CanRequestResetAsync(string email)
        {
            var lastRequest = await _context.PasswordResetTokens
                .Where(t => t.Email == email)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastRequest == null)
                return true;

            return DateTime.UtcNow >= lastRequest.CreatedAt.AddMinutes(RATE_LIMIT_MINUTES);
        }

        public async Task CleanupExpiredTokensAsync()
        {
            try
            {
                var expiredTokens = await _context.PasswordResetTokens
                    .Where(t => t.ExpiresAt < DateTime.UtcNow || t.CreatedAt < DateTime.UtcNow.AddDays(-7))
                    .ToListAsync();

                if (expiredTokens.Any())
                {
                    _context.PasswordResetTokens.RemoveRange(expiredTokens);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cleaned up {Count} expired password reset tokens", expiredTokens.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired tokens");
            }
        }

        private string GenerateResetCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string HashResetCode(string resetCode)
        {
            using var sha256 = SHA256.Create();
            var salt = _configuration["Security:PasswordResetSalt"] ?? "DefaultSalt2024!";
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(resetCode + salt));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
