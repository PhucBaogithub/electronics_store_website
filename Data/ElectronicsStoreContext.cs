using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Models;

namespace ElectronicsStore.Data
{
    public class ElectronicsStoreContext : IdentityDbContext<User>
    {
        public ElectronicsStoreContext(DbContextOptions<ElectronicsStoreContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductReview>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.ProductReviews)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductReview>()
                .HasOne(pr => pr.Product)
                .WithMany(p => p.ProductReviews)
                .HasForeignKey(pr => pr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure decimal precision
            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Entity<Product>()
                .Property(p => p.DiscountPrice)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(o => o.ShippingCost)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(o => o.Tax)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(o => o.GrandTotal)
                .HasPrecision(18, 2);

            builder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            builder.Entity<OrderItem>()
                .Property(oi => oi.TotalPrice)
                .HasPrecision(18, 2);

            // Configure unique constraints
            builder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            builder.Entity<CartItem>()
                .HasIndex(ci => new { ci.UserId, ci.ProductId })
                .IsUnique();

            builder.Entity<ProductReview>()
                .HasIndex(pr => new { pr.UserId, pr.ProductId })
                .IsUnique();

            // Seed data
            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Processors", Description = "CPUs and microprocessors" },
                new Category { Id = 2, Name = "Memory", Description = "RAM and storage devices" },
                new Category { Id = 3, Name = "Graphics Cards", Description = "GPUs and video cards" },
                new Category { Id = 4, Name = "Motherboards", Description = "Computer motherboards and mainboards" },
                new Category { Id = 5, Name = "Storage", Description = "Hard drives, SSDs, and storage solutions" },
                new Category { Id = 6, Name = "Power Supplies", Description = "PSUs and power management" },
                new Category { Id = 7, Name = "Cooling", Description = "Fans, coolers, and thermal solutions" },
                new Category { Id = 8, Name = "Cases", Description = "Computer cases and chassis" }
            );

            // Seed Products
            builder.Entity<Product>().HasData(
                // Processors
                new Product { Id = 1, Name = "Intel Core i7-12700K", Description = "12th Gen Intel Core processor", Price = 399.99m, StockQuantity = 50, CategoryId = 1, Brand = "Intel", Model = "i7-12700K", IsFeatured = true },
                new Product { Id = 2, Name = "AMD Ryzen 7 5800X", Description = "8-core, 16-thread desktop processor", Price = 299.99m, StockQuantity = 30, CategoryId = 1, Brand = "AMD", Model = "5800X", IsFeatured = true },
                
                // Memory
                new Product { Id = 3, Name = "Corsair Vengeance LPX 16GB DDR4", Description = "High-performance DDR4 memory", Price = 79.99m, StockQuantity = 100, CategoryId = 2, Brand = "Corsair", Model = "CMK16GX4M2B3200C16" },
                new Product { Id = 4, Name = "G.Skill Trident Z RGB 32GB DDR4", Description = "RGB DDR4 memory kit", Price = 149.99m, StockQuantity = 25, CategoryId = 2, Brand = "G.Skill", Model = "F4-3200C16D-32GTZR" },
                
                // Graphics Cards
                new Product { Id = 5, Name = "NVIDIA RTX 4070", Description = "High-performance graphics card", Price = 599.99m, DiscountPrice = 549.99m, StockQuantity = 15, CategoryId = 3, Brand = "NVIDIA", Model = "RTX 4070", IsFeatured = true },
                new Product { Id = 6, Name = "AMD Radeon RX 7800 XT", Description = "RDNA 3 graphics card", Price = 499.99m, StockQuantity = 20, CategoryId = 3, Brand = "AMD", Model = "RX 7800 XT" },
                
                // Motherboards
                new Product { Id = 7, Name = "ASUS ROG Strix B550-F Gaming", Description = "ATX gaming motherboard", Price = 189.99m, StockQuantity = 40, CategoryId = 4, Brand = "ASUS", Model = "ROG-STRIX-B550-F-GAMING" },
                new Product { Id = 8, Name = "MSI MPG Z690 Carbon", Description = "Intel Z690 motherboard", Price = 299.99m, StockQuantity = 25, CategoryId = 4, Brand = "MSI", Model = "MPG-Z690-CARBON-WIFI" },
                
                // Storage
                new Product { Id = 9, Name = "Samsung 980 PRO 1TB NVMe SSD", Description = "High-speed NVMe SSD", Price = 129.99m, StockQuantity = 60, CategoryId = 5, Brand = "Samsung", Model = "MZ-V8P1T0B/AM", IsFeatured = true },
                new Product { Id = 10, Name = "Western Digital Black 2TB HDD", Description = "High-performance hard drive", Price = 89.99m, StockQuantity = 80, CategoryId = 5, Brand = "Western Digital", Model = "WD2003FZEX" }
            );
        }
    }
} 