using ElectronicsStore.Models.DTOs;
using ElectronicsStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ElectronicsStore.Controllers
{
    [AllowAnonymous]
    public class ForgotPasswordController : Controller
    {
        private readonly IPasswordResetService _passwordResetService;
        private readonly ILogger<ForgotPasswordController> _logger;

        public ForgotPasswordController(
            IPasswordResetService passwordResetService,
            ILogger<ForgotPasswordController> logger)
        {
            _passwordResetService = passwordResetService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ForgotPasswordDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var ipAddress = GetClientIpAddress();
                var userAgent = Request.Headers["User-Agent"].ToString();

                var result = await _passwordResetService.SendResetCodeAsync(
                    model.Email, 
                    ipAddress, 
                    userAgent
                );

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    TempData["Email"] = model.Email;
                    TempData["ExpiresAt"] = result.ExpiresAt?.ToString("yyyy-MM-dd HH:mm:ss");
                    return RedirectToAction("ResetPassword");
                }
                else
                {
                    if (result.RetryAfter.HasValue)
                    {
                        TempData["ErrorMessage"] = result.Message;
                        TempData["RetryAfter"] = result.RetryAfter.Value.TotalSeconds;
                    }
                    else
                    {
                        ModelState.AddModelError("", result.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password for email: {Email}", model.Email);
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại sau.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var model = new ResetPasswordDTO();
            
            if (TempData["Email"] != null)
            {
                model.Email = TempData["Email"].ToString() ?? "";
                TempData.Keep("Email");
            }

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            if (TempData["ExpiresAt"] != null)
            {
                ViewBag.ExpiresAt = TempData["ExpiresAt"].ToString();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // First verify the reset code
                var verifyResult = await _passwordResetService.VerifyResetCodeAsync(
                    model.Email, 
                    model.ResetCode
                );

                if (!verifyResult.Success)
                {
                    ModelState.AddModelError("ResetCode", verifyResult.Message);
                    return View(model);
                }

                // Reset the password
                var ipAddress = GetClientIpAddress();
                var userAgent = Request.Headers["User-Agent"].ToString();

                var resetResult = await _passwordResetService.ResetPasswordAsync(
                    model.Email,
                    model.ResetCode,
                    model.NewPassword,
                    ipAddress,
                    userAgent
                );

                if (resetResult.Success)
                {
                    TempData["SuccessMessage"] = resetResult.Message;
                    return RedirectToAction("Success");
                }
                else
                {
                    if (resetResult.Errors.Any())
                    {
                        foreach (var error in resetResult.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", resetResult.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for email: {Email}", model.Email);
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại sau.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Success()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code))
                {
                    return Json(new { success = false, message = "Email và mã xác nhận là bắt buộc." });
                }

                var result = await _passwordResetService.VerifyResetCodeAsync(request.Email, request.Code);
                
                return Json(new { 
                    success = result.Success, 
                    message = result.Message 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying reset code");
                return Json(new { success = false, message = "Có lỗi xảy ra. Vui lòng thử lại." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResendCode([FromBody] ResendCodeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return Json(new { success = false, message = "Email là bắt buộc." });
                }

                var canRequest = await _passwordResetService.CanRequestResetAsync(request.Email);
                if (!canRequest)
                {
                    return Json(new { success = false, message = "Vui lòng đợi 5 phút trước khi gửi lại mã." });
                }

                var ipAddress = GetClientIpAddress();
                var userAgent = Request.Headers["User-Agent"].ToString();

                var result = await _passwordResetService.SendResetCodeAsync(
                    request.Email, 
                    ipAddress, 
                    userAgent
                );

                return Json(new { 
                    success = result.Success, 
                    message = result.Message,
                    expiresAt = result.ExpiresAt?.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending reset code");
                return Json(new { success = false, message = "Có lỗi xảy ra. Vui lòng thử lại." });
            }
        }

        [HttpGet]
        public IActionResult TestEmailPage()
        {
            return View("TestEmail");
        }

        [HttpGet]
        public async Task<IActionResult> TestEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email is required" });
            }

            try
            {
                var emailService = HttpContext.RequestServices.GetService<ElectronicsStore.Services.IEmailService>();
                if (emailService == null)
                {
                    return Json(new { success = false, message = "Email service not available" });
                }

                var result = await emailService.SendTestEmailAsync(email);
                return Json(new {
                    success = result,
                    message = result ? "Test email sent successfully!" : "Failed to send test email",
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email to {Email}", email);
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private string GetClientIpAddress()
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress;

                if (ipAddress != null)
                {
                    if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        ipAddress = ipAddress.MapToIPv4();
                    }
                    return ipAddress.ToString();
                }

                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }

    public class VerifyCodeRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class ResendCodeRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
