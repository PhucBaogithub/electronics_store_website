using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ElectronicsStore.Models;

namespace ElectronicsStore.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<User>? _userManager;

        public BaseController(UserManager<User>? userManager = null)
        {
            _userManager = userManager;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (_userManager != null && User.Identity?.IsAuthenticated == true)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                ViewData["CurrentUser"] = currentUser;
            }

            await next();
        }
    }
} 