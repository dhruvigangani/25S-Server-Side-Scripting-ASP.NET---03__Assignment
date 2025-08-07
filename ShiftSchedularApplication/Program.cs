using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularApplication.Data;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

var configConnection = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine($"Config Connection: {(string.IsNullOrEmpty(configConnection) ? "NOT SET" : "SET")}");

// Determine which database to use based on environment
var isProduction = builder.Environment.IsProduction();
var connectionString = configConnection ?? "Data Source=app.db";

Console.WriteLine($"Environment: {(isProduction ? "Production" : "Development")}");
Console.WriteLine($"Connection string provided: {!string.IsNullOrEmpty(configConnection)}");

if (isProduction && !string.IsNullOrEmpty(configConnection))
{
    // Use PostgreSQL in production
    Console.WriteLine("Using PostgreSQL database");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // Use SQLite in development
    Console.WriteLine("Using SQLite database");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8; // Increased from 6 to 8 for better security
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Lock account for 15 minutes
    options.Lockout.MaxFailedAccessAttempts = 5; // Lock after 5 failed attempts
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configure cookie settings for better OAuth support
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromHours(4); // Reduced from 12 to 4 hours for better security
    options.SlidingExpiration = true;
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Configure external cookie for OAuth
builder.Services.ConfigureExternalCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
});

var authenticationBuilder = builder.Services.AddAuthentication();

// Only add Google authentication if ClientId is provided
var googleAuth = builder.Configuration.GetSection("Authentication:Google");
var clientId = googleAuth["ClientId"];
Console.WriteLine($"Google ClientId: {(string.IsNullOrEmpty(clientId) ? "NOT SET" : "SET")}");
if (!string.IsNullOrEmpty(clientId))
{
    Console.WriteLine("Adding Google authentication");
    authenticationBuilder.AddGoogle(options =>
    {
        options.ClientId = clientId;
        options.ClientSecret = googleAuth["ClientSecret"] ?? string.Empty;
        options.CallbackPath = "/signin-google";
        options.SaveTokens = true;
        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });
}
else
{
    Console.WriteLine("Google authentication not configured");
}

// Only add Facebook authentication if AppId is provided
var facebookAuth = builder.Configuration.GetSection("Authentication:Facebook");
var facebookAppId = facebookAuth["AppId"];
if (!string.IsNullOrEmpty(facebookAppId) && facebookAppId != "YOUR_FACEBOOK_APP_ID")
{
    authenticationBuilder.AddFacebook(options =>
    {
        options.AppId = facebookAppId;
        options.AppSecret = facebookAuth["AppSecret"] ?? string.Empty;
        options.CallbackPath = "/signin-facebook";
    });
}

builder.Services.AddControllersWithViews();

// Configure DataProtection for containerized environments
// Use the default configuration which works well in containers
builder.Services.AddDataProtection()
    .SetApplicationName("ShiftSchedularApplication");

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    try
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        Console.WriteLine("Attempting to create database...");
        
        // For PostgreSQL, we need to ensure the database exists
        if (isProduction && !string.IsNullOrEmpty(configConnection))
        {
            // PostgreSQL specific initialization
            Console.WriteLine("Initializing PostgreSQL database...");
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("PostgreSQL database initialized successfully!");
            
            // Verify tables were created
            var tables = await context.Database.SqlQueryRaw<string>("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'").ToListAsync();
            Console.WriteLine($"Tables created: {string.Join(", ", tables)}");
        }
        else
        {
            // SQLite initialization
            Console.WriteLine("Initializing SQLite database...");
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("SQLite database created successfully!");
        }
        
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        await SeedUsersAsync(userManager);
        Console.WriteLine("User seeding completed!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during database initialization: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        
        // Log specific database errors
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
        
        // Don't throw - database initialization failure shouldn't stop the app
        // The app can still run without the database being fully initialized
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configure HTTPS redirection only in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
{
    try
    {
        var user = await userManager.FindByEmailAsync("dario@gc.ca");
        if (user == null)
        {
            var newUser = new IdentityUser
            {
                UserName = "dario@gc.ca",
                Email = "dario@gc.ca",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(newUser, "Test123$!");
            if (!result.Succeeded)
            {
                Console.WriteLine($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            else
            {
                Console.WriteLine("User 'dario@gc.ca' created successfully!");
            }
        }
        else
        {
            // Update existing user's password to meet new requirements
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await userManager.ResetPasswordAsync(user, token, "Test123$!");
            if (resetResult.Succeeded)
            {
                Console.WriteLine("User 'dario@gc.ca' password updated successfully!");
            }
            else
            {
                Console.WriteLine($"Failed to update user password: {string.Join(", ", resetResult.Errors.Select(e => e.Description))}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during user seeding: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        // Don't throw - user seeding failure shouldn't stop the app
    }
}