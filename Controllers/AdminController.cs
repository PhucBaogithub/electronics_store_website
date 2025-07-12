using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;

namespace ElectronicsStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ElectronicsStoreContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ElectronicsStoreContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = new
            {
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                ActiveProducts = await _context.Products.CountAsync(p => p.IsActive),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending),
                LowStockProducts = await _context.Products.CountAsync(p => p.StockQuantity < 10 && p.IsActive),
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .ToListAsync()
            };

            ViewBag.DashboardData = dashboardData;
            ViewBag.TotalReviews = await _context.ProductReviews.CountAsync();
            return View();
        }

        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            
            return View(products);
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
            
            return View(categories);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var usersWithRoles = new List<(User User, IList<string> Roles)>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add((user, roles));
            }

            return View(usersWithRoles);
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            
            return View(orders);
        }

        public async Task<IActionResult> Reviews()
        {
            var reviews = await _context.ProductReviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .ThenInclude(p => p.Category)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                var review = await _context.ProductReviews.FindAsync(id);
                if (review == null)
                {
                    TempData["Error"] = "Không tìm thấy đánh giá.";
                    return RedirectToAction(nameof(Reviews));
                }

                _context.ProductReviews.Remove(review);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Đánh giá đã được xóa thành công!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", id);
                TempData["Error"] = "Có lỗi xảy ra khi xóa đánh giá.";
            }

            return RedirectToAction(nameof(Reviews));
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    product.CreatedAt = DateTime.UtcNow;
                    product.UpdatedAt = DateTime.UtcNow;
                    
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Sản phẩm đã được tạo thành công!";
                    return RedirectToAction(nameof(Products));
                }
                else
                {
                    _logger.LogWarning("ModelState is invalid for CreateProduct:");
                    foreach (var modelState in ModelState)
                    {
                        foreach (var error in modelState.Value.Errors)
                        {
                            _logger.LogWarning($"Field: {modelState.Key}, Error: {error.ErrorMessage}");
                        }
                    }
                    TempData["Error"] = "Vui lòng kiểm tra lại thông tin đã nhập.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                TempData["Error"] = "Có lỗi xảy ra khi tạo sản phẩm. Vui lòng thử lại.";
            }

            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
            
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.DiscountPrice = product.DiscountPrice;
                    existingProduct.StockQuantity = product.StockQuantity;
                    existingProduct.ImageUrl = product.ImageUrl;
                    existingProduct.Brand = product.Brand;
                    existingProduct.Model = product.Model;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.IsActive = product.IsActive;
                    existingProduct.IsFeatured = product.IsFeatured;
                    existingProduct.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sản phẩm đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Products));
                }
                else
                {
                    _logger.LogWarning("ModelState is invalid for EditProduct:");
                    foreach (var modelState in ModelState)
                    {
                        foreach (var error in modelState.Value.Errors)
                        {
                            _logger.LogWarning($"Field: {modelState.Key}, Error: {error.ErrorMessage}");
                        }
                    }
                    TempData["Error"] = "Vui lòng kiểm tra lại thông tin đã nhập.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật sản phẩm.";
            }

            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.CartItems)
                    .Include(p => p.OrderItems)
                    .Include(p => p.ProductReviews)
                    .FirstOrDefaultAsync(p => p.Id == id);
                    
                if (product != null)
                {
                    // Remove related data first
                    _context.CartItems.RemoveRange(product.CartItems);
                    _context.OrderItems.RemoveRange(product.OrderItems);
                    _context.ProductReviews.RemoveRange(product.ProductReviews);
                    
                    // Hard delete - remove completely from database
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Sản phẩm đã được xóa hoàn toàn khỏi hệ thống!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm cần xóa.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                TempData["Error"] = "Có lỗi xảy ra khi xóa sản phẩm. Vui lòng thử lại.";
            }

            return RedirectToAction(nameof(Products));
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedAt = DateTime.UtcNow;
                
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Danh mục đã được tạo thành công!";
                return RedirectToAction(nameof(Categories));
            }

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Danh mục đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Categories));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating category {CategoryId}", id);
                    TempData["Error"] = "Có lỗi xảy ra khi cập nhật danh mục.";
                }
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                // Check if category has products
                var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
                if (hasProducts)
                {
                    TempData["Error"] = "Không thể xóa danh mục có sản phẩm. Vui lòng di chuyển sản phẩm sang danh mục khác trước.";
                }
                else
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Danh mục đã được xóa thành công!";
                }
            }

            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Đơn hàng không tồn tại" });
                }

                if (Enum.TryParse<OrderStatus>(status, out var orderStatus))
                {
                    order.Status = orderStatus;
                    await _context.SaveChangesAsync();
                    
                    return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Trạng thái không hợp lệ" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for order {OrderId}", orderId);
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserDetail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRoles = roles;
            
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Người dùng không tồn tại" });
                }

                user.IsActive = !user.IsActive;
                var result = await _userManager.UpdateAsync(user);
                
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Cập nhật trạng thái thành công", isActive = user.IsActive });
                }
                else
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status for user {UserId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
        }
    }
} 