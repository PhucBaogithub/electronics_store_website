using System.Net;
using System.Net.Mail;
using System.Text;

namespace ElectronicsStore.Services
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetCode, string userName);
        Task<bool> SendPasswordResetSuccessEmailAsync(string toEmail, string userName);
        Task<bool> SendTestEmailAsync(string toEmail);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetCode, string userName)
        {
            try
            {
                var subject = "Đặt lại mật khẩu - Electronics Store";
                var body = GeneratePasswordResetEmailBody(resetCode, userName);

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetSuccessEmailAsync(string toEmail, string userName)
        {
            try
            {
                var subject = "Mật khẩu đã được đặt lại thành công - Electronics Store";
                var body = GeneratePasswordResetSuccessEmailBody(userName);

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset success email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendTestEmailAsync(string toEmail)
        {
            try
            {
                var subject = "🧪 Test Email - Electronics Store";
                var body = GenerateTestEmailBody();

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email to {Email}", toEmail);
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("EmailSettings");
                var smtpHost = smtpSettings["SmtpHost"];
                var smtpPort = int.Parse(smtpSettings["SmtpPort"] ?? "587");
                var smtpUsername = smtpSettings["SmtpUsername"];
                var smtpPassword = smtpSettings["SmtpPassword"];
                var fromEmail = smtpSettings["FromEmail"];
                var fromName = smtpSettings["FromName"];

                // Validate configuration
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogError("Email configuration is incomplete. Please check appsettings.json");
                    return false;
                }

                _logger.LogInformation("Attempting to send email to {Email} via {SmtpHost}:{SmtpPort}", toEmail, smtpHost, smtpPort);

                using var client = new SmtpClient(smtpHost, smtpPort);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.Timeout = 30000; // 30 seconds timeout

                using var message = new MailMessage();
                message.From = new MailAddress(fromEmail ?? smtpUsername ?? "", fromName ?? "Electronics Store");
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                message.SubjectEncoding = Encoding.UTF8;

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP error sending email to {Email}: {StatusCode}", toEmail, smtpEx.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }

        private string GeneratePasswordResetEmailBody(string resetCode, string userName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Đặt lại mật khẩu</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
        .reset-code {{ background: #007bff; color: white; font-size: 32px; font-weight: bold; padding: 20px; text-align: center; border-radius: 8px; margin: 20px 0; letter-spacing: 8px; }}
        .warning {{ background: #fff3cd; border: 1px solid #ffeaa7; color: #856404; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 14px; }}
        .button {{ display: inline-block; background: #007bff; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 10px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Đặt lại mật khẩu</h1>
            <p>Electronics Store</p>
        </div>
        <div class='content'>
            <h2>Xin chào {userName}!</h2>
            <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
            
            <p><strong>Mã xác nhận của bạn là:</strong></p>
            <div class='reset-code'>{resetCode}</div>
            
            <div class='warning'>
                <strong>⚠️ Lưu ý quan trọng:</strong>
                <ul>
                    <li>Mã này chỉ có hiệu lực trong <strong>15 phút</strong></li>
                    <li>Mã chỉ có thể sử dụng <strong>một lần duy nhất</strong></li>
                    <li>Không chia sẻ mã này với bất kỳ ai</li>
                </ul>
            </div>
            
            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này. Tài khoản của bạn vẫn an toàn.</p>
            
            <p>Nếu bạn cần hỗ trợ, vui lòng liên hệ với chúng tôi qua email: <strong>support@electronicsstore.com</strong></p>
        </div>
        <div class='footer'>
            <p>© 2024 Electronics Store. Tất cả quyền được bảo lưu.</p>
            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GeneratePasswordResetSuccessEmailBody(string userName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Mật khẩu đã được đặt lại</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
        .success-icon {{ font-size: 48px; color: #28a745; text-align: center; margin: 20px 0; }}
        .info-box {{ background: #d1ecf1; border: 1px solid #bee5eb; color: #0c5460; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 14px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✅ Thành công!</h1>
            <p>Electronics Store</p>
        </div>
        <div class='content'>
            <div class='success-icon'>🎉</div>
            <h2>Xin chào {userName}!</h2>
            <p>Mật khẩu của bạn đã được đặt lại thành công vào lúc <strong>{DateTime.Now:dd/MM/yyyy HH:mm}</strong>.</p>

            <div class='info-box'>
                <strong>🔒 Bảo mật tài khoản:</strong>
                <ul>
                    <li>Hãy đăng nhập bằng mật khẩu mới</li>
                    <li>Không chia sẻ mật khẩu với ai</li>
                    <li>Sử dụng mật khẩu mạnh và duy nhất</li>
                    <li>Cập nhật mật khẩu định kỳ</li>
                </ul>
            </div>

            <p>Nếu bạn không thực hiện thay đổi này, vui lòng liên hệ ngay với chúng tôi qua:</p>
            <ul>
                <li>Email: <strong>support@electronicsstore.com</strong></li>
                <li>Hotline: <strong>(028) 1234-5678</strong></li>
            </ul>
        </div>
        <div class='footer'>
            <p>© 2024 Electronics Store. Tất cả quyền được bảo lưu.</p>
            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GenerateTestEmailBody()
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Test Email</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #007bff 0%, #0056b3 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
        .test-info {{ background: #d4edda; border: 1px solid #c3e6cb; color: #155724; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 14px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🧪 Test Email</h1>
            <p>Electronics Store - Email Configuration Test</p>
        </div>
        <div class='content'>
            <h2>Email Configuration Test Successful!</h2>
            <p>Congratulations! Your Gmail SMTP configuration is working correctly.</p>

            <div class='test-info'>
                <strong>✅ Test Details:</strong>
                <ul>
                    <li>Timestamp: <strong>{DateTime.Now:dd/MM/yyyy HH:mm:ss}</strong></li>
                    <li>SMTP Server: <strong>smtp.gmail.com:587</strong></li>
                    <li>Encryption: <strong>TLS/SSL</strong></li>
                    <li>Status: <strong>SUCCESS</strong></li>
                </ul>
            </div>

            <p><strong>Next Steps:</strong></p>
            <ol>
                <li>Test the complete password reset flow</li>
                <li>Verify email templates display correctly</li>
                <li>Test rate limiting functionality</li>
                <li>Check error handling scenarios</li>
            </ol>

            <p>Your Electronics Store password reset feature is ready to use!</p>
        </div>
        <div class='footer'>
            <p>© 2024 Electronics Store. All rights reserved.</p>
            <p>This is an automated test email.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
