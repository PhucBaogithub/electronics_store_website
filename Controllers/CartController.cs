using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;
using ElectronicsStore.Models.DTOs;

namespace ElectronicsStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ElectronicsStoreContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ElectronicsStoreContext context,
            UserManager<User> userManager,
            ILogger<CartController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string? returnUrl = null)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    // Redirect to login with return URL to this product
                    var loginUrl = Url.Action("Login", "Account", new { 
                        returnUrl = returnUrl ?? Url.Action("ProductDetail", "Home", new { id = productId })
                    });
                    return Redirect(loginUrl ?? "/Account/Login");
                }

                var product = await _context.Products.FindAsync(productId);
                if (product == null || !product.IsActive)
                {
                    TempData["Error"] = "Sản phẩm không tồn tại hoặc đã ngừng bán.";
                    return RedirectToAction("Products", "Home");
                }

                if (product.StockQuantity < quantity)
                {
                    TempData["Error"] = "Số lượng sản phẩm trong kho không đủ.";
                    return RedirectToAction("ProductDetail", "Home", new { id = productId });
                }

                // Check if item already exists in cart
                var existingCartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ProductId == productId);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
                    existingCartItem.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        UserId = user.Id,
                        ProductId = productId,
                        Quantity = quantity,
                        Price = product.Price,
                        DateAdded = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.CartItems.Add(cartItem);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";

                // Always redirect to cart after successful add
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart for user {UserId}", productId, _userManager.GetUserId(User));
                TempData["Error"] = "Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("ProductDetail", "Home", new { id = productId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login", "Account");

                var cartItem = await _context.CartItems
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == user.Id);

                if (cartItem == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm trong giỏ hàng.";
                    return RedirectToAction("Index");
                }

                if (quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                    TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
                }
                else if (quantity > cartItem.Product.StockQuantity)
                {
                    TempData["Error"] = "Số lượng vượt quá số hàng có sẵn.";
                    return RedirectToAction("Index");
                }
                else
                {
                    cartItem.Quantity = quantity;
                    cartItem.UpdatedAt = DateTime.UtcNow;
                    TempData["Success"] = "Đã cập nhật số lượng sản phẩm.";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartItemId} quantity", cartItemId);
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật giỏ hàng.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login", "Account");

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == user.Id);

                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {CartItemId}", cartItemId);
                TempData["Error"] = "Có lỗi xảy ra khi xóa sản phẩm.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { count = 0 });

            var count = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .SumAsync(c => c.Quantity);

            return Json(new { count = count });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not authenticated" });

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .Select(c => new
                {
                    CartItemId = c.Id,
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Product = new
                    {
                        Id = c.Product.Id,
                        Name = c.Product.Name,
                        Brand = c.Product.Brand,
                        Price = c.Product.Price,
                        DiscountPrice = c.Product.DiscountPrice,
                        FinalPrice = c.Product.FinalPrice,
                        ImageUrl = !string.IsNullOrEmpty(c.Product.ImageUrl) ? c.Product.ImageUrl : "https://via.placeholder.com/50x50/007bff/ffffff?text=No+Image"
                    }
                })
                .ToListAsync();

            return Json(new { success = true, items = cartItems });
        }
    }
} 