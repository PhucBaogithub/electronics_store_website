using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Models;
using ElectronicsStore.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicsStore.Controllers
{
    public class DemoOrderRequest
    {
        public DemoShippingAddress? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
    }

    public class DemoShippingAddress
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
    }

    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ElectronicsStoreContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, ElectronicsStoreContext context) 
            : base(userManager)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get featured products
            var featuredProducts = await _context.Products
                .Where(p => p.IsActive && p.IsFeatured)
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .Take(6)
                .ToListAsync();

            ViewBag.FeaturedProducts = featuredProducts;
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }

        public IActionResult ProductDetail(int id)
        {
            ViewBag.ProductId = id;
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Admin()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Orders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProcessDemoOrder([FromBody] DemoOrderRequest request)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Get cart items from database
                var cartItems = await _context.CartItems
                    .Include(c => c.Product)
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return Json(new { success = false, message = "Cart is empty" });
                }

                // Calculate totals from actual cart
                var subtotal = cartItems.Sum(c => c.Product.FinalPrice * c.Quantity);
                var tax = subtotal * 0.1m; // 10% tax
                var shippingCost = 0m; // Free shipping
                var grandTotal = subtotal + tax + shippingCost;

                // Create order from cart
                var order = new Order
                {
                    UserId = user.Id,
                    OrderNumber = "ORD-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending,
                    PaymentStatus = PaymentStatus.Paid,
                    
                    // Shipping information
                    ShippingFirstName = request.ShippingAddress?.FirstName ?? user.FirstName,
                    ShippingLastName = request.ShippingAddress?.LastName ?? user.LastName,
                    ShippingAddress = request.ShippingAddress?.Address ?? "Default Address",
                    ShippingCity = request.ShippingAddress?.City ?? "Ho Chi Minh City",
                    ShippingPostalCode = request.ShippingAddress?.ZipCode ?? "70000",
                    ShippingCountry = "Vietnam",
                    ShippingPhone = request.ShippingAddress?.Phone ?? user.PhoneNumber ?? "0123456789",
                    
                    // Calculated totals
                    TotalAmount = subtotal,
                    ShippingCost = shippingCost,
                    Tax = tax,
                    GrandTotal = grandTotal,
                    
                    Notes = request.Notes
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create order items from cart items
                var orderItems = cartItems.Select(c => new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Product.FinalPrice,
                    TotalPrice = c.Product.FinalPrice * c.Quantity
                }).ToList();

                _context.OrderItems.AddRange(orderItems);

                // Clear cart after successful order
                _context.CartItems.RemoveRange(cartItems);
                
                await _context.SaveChangesAsync();

                return Json(new { success = true, orderId = order.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order");
                return Json(new { success = false, message = "Error processing order" });
            }
        }

        [Authorize]
        public async Task<IActionResult> OrderDetail(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

                if (order == null)
                {
                    return Json(new { success = false, message = "Order not found" });
                }

                // Only allow cancellation for pending orders
                if (order.Status != OrderStatus.Pending)
                {
                    return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang chờ xử lý" });
                }

                order.Status = OrderStatus.Cancelled;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đơn hàng đã được hủy thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi hủy đơn hàng" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
} 