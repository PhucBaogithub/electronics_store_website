using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;

namespace ElectronicsStore.Controllers
{
    [Route("Products")]
    public class ProductViewController : Controller
    {
        private readonly ElectronicsStoreContext _context;
        private readonly ILogger<ProductViewController> _logger;

        public ProductViewController(ElectronicsStoreContext context, ILogger<ProductViewController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Display product detail page
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product detail view</returns>
        [Route("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

                if (product == null)
                {
                    return NotFound();
                }

                ViewBag.ProductId = id;
                return View("~/Views/Home/ProductDetail.cshtml", product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product details for product {ProductId}", id);
                return NotFound();
            }
        }

        /// <summary>
        /// Display products listing page
        /// </summary>
        /// <returns>Products listing view</returns>
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View("~/Views/Home/Products.cshtml");
        }
    }
}
