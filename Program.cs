using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework with SQLite
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=electronicsstore.db"));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    
    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ElectronicsStoreContext>()
.AddDefaultTokenProviders();

// Configure Cookie Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Chatbot Service
builder.Services.AddScoped<ElectronicsStore.Services.IChatbotService, ElectronicsStore.Services.ChatbotService>();

// Add Email Service
builder.Services.AddScoped<ElectronicsStore.Services.IEmailService, ElectronicsStore.Services.EmailService>();

// Add Password Reset Service
builder.Services.AddScoped<ElectronicsStore.Services.IPasswordResetService, ElectronicsStore.Services.PasswordResetService>();

// Add Background Services
builder.Services.AddHostedService<ElectronicsStore.Services.PasswordResetCleanupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Configure routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Health check endpoint for Docker
app.MapGet("/health", () => Results.Ok(new {
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ElectronicsStoreContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Ensure database is created
        context.Database.EnsureCreated();
        
        // Seed roles and admin user
        await SeedData(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

async Task SeedData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
{
    // Create roles
    string[] roles = { "Admin", "User" };
    
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
    
    // Create admin user
    var adminEmail = "admin@electronicsstore.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true,
            IsActive = true
        };
        
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    
    // Create test user
    var testEmail = "user@test.com";
    var testUser = await userManager.FindByEmailAsync(testEmail);
    
    if (testUser == null)
    {
        testUser = new User
        {
            UserName = testEmail,
            Email = testEmail,
            FirstName = "Test",
            LastName = "User",
            EmailConfirmed = true,
            IsActive = true
        };
        
        var result = await userManager.CreateAsync(testUser, "Test123!");
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(testUser, "User");
        }
    }
}

app.Run();