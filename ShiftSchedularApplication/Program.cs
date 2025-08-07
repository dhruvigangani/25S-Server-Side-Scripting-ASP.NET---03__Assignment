using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularApplication.Data;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
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
    options.Cookie.SecurePolicy = builder.Environment.IsProduction() ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromHours(4);
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
    options.Cookie.SecurePolicy = builder.Environment.IsProduction() ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
});

var authenticationBuilder = builder.Services.AddAuthentication();

// Only add Google authentication if ClientId is provided
var googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? builder.Configuration["Authentication:Google:ClientId"];
Console.WriteLine($"Google ClientId: {(string.IsNullOrEmpty(googleClientId) ? "NOT SET" : "SET")}");
if (!string.IsNullOrEmpty(googleClientId))
{
    Console.WriteLine("Adding Google authentication");
    authenticationBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
        options.CallbackPath = "/signin-google";
        options.SaveTokens = true;
        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        options.CorrelationCookie.SecurePolicy = builder.Environment.IsProduction() ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
        options.CorrelationCookie.HttpOnly = true;
        options.CorrelationCookie.MaxAge = TimeSpan.FromMinutes(5);
    });
}
else
{
    Console.WriteLine("Google authentication not configured");
}

// Only add Facebook authentication if AppId is provided
var facebookAppId = Environment.GetEnvironmentVariable("FACEBOOK_APP_ID") ?? builder.Configuration["Authentication:Facebook:AppId"];
if (!string.IsNullOrEmpty(facebookAppId) && facebookAppId != "YOUR_FACEBOOK_APP_ID")
{
    authenticationBuilder.AddFacebook(options =>
    {
        options.AppId = facebookAppId;
        options.AppSecret = Environment.GetEnvironmentVariable("FACEBOOK_APP_SECRET") ?? builder.Configuration["Authentication:Facebook:AppSecret"] ?? string.Empty;
        options.CallbackPath = "/signin-facebook";
        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        options.CorrelationCookie.SecurePolicy = builder.Environment.IsProduction() ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
        options.CorrelationCookie.HttpOnly = true;
        options.CorrelationCookie.MaxAge = TimeSpan.FromMinutes(5);
    });
}

builder.Services.AddControllersWithViews();

// Configure DataProtection for containerized environments
var dataProtectionBuilder = builder.Services.AddDataProtection()
    .SetApplicationName("ShiftSchedularApplication");

// Configure data protection based on environment
if (builder.Environment.IsProduction())
{
    // In production, use file system for key persistence
    var keysDirectory = new DirectoryInfo("/tmp/keys");
    if (!keysDirectory.Exists)
    {
        keysDirectory.Create();
    }
    dataProtectionBuilder.PersistKeysToFileSystem(keysDirectory);
}
else
{
    // In development, use default configuration
    dataProtectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "keys")));
}

dataProtectionBuilder.SetDefaultKeyLifetime(TimeSpan.FromDays(90));

var app = builder.Build();

// Ensure data protection keys directory exists
if (app.Environment.IsProduction())
{
    try
    {
        var keysDirectory = new DirectoryInfo("/tmp/keys");
        if (!keysDirectory.Exists)
        {
            keysDirectory.Create();
            Console.WriteLine("Created data protection keys directory: /tmp/keys");
        }
        Console.WriteLine($"Data protection keys directory exists: {keysDirectory.Exists}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Warning: Could not create data protection keys directory: {ex.Message}");
    }
}

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

// Configure HTTPS redirection and security
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    
    // Force HTTPS for OAuth providers
    app.Use(async (context, next) =>
    {
        if (!context.Request.IsHttps && !context.Request.Headers["X-Forwarded-Proto"].Contains("https"))
        {
            var httpsUrl = $"https://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
            context.Response.Redirect(httpsUrl, permanent: true);
            return;
        }
        await next();
    });
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

// Add OAuth error handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (AuthenticationFailureException ex) when (ex.Message.Contains("oauth state"))
    {
        // Redirect to login page with error message
        context.Response.Redirect("/Identity/Account/Login?error=oauth_state_invalid");
        return;
    }
    catch (Exception ex)
    {
        // Log OAuth errors
        if (context.Request.Path.StartsWithSegments("/signin-google") || context.Request.Path.StartsWithSegments("/signin-facebook"))
        {
            Console.WriteLine($"OAuth error: {ex.Message}");
            context.Response.Redirect("/Identity/Account/Login?error=oauth_failed");
            return;
        }
        throw;
    }
});

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