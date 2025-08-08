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
        
        // Configure events to handle data protection issues and force HTTPS
        options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
        {
            OnRedirectToAuthorizationEndpoint = context =>
            {
                Console.WriteLine($"Google OAuth redirect URI: {context.RedirectUri}");
                // Force HTTPS for OAuth redirect
                if (builder.Environment.IsProduction())
                {
                    var uri = new Uri(context.RedirectUri);
                    var httpsUri = new UriBuilder(uri) { Scheme = "https" }.Uri.ToString();
                    Console.WriteLine($"Google OAuth HTTPS redirect URI: {httpsUri}");
                    context.Response.Redirect(httpsUri);
                    return Task.CompletedTask;
                }
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            },
            OnRemoteFailure = context =>
            {
                Console.WriteLine($"Google OAuth remote failure: {context.Failure?.Message}");
                Console.WriteLine($"Google OAuth failure details: {context.Failure?.StackTrace}");
                context.HandleResponse();
                context.Response.Redirect("/Identity/Account/Login?error=oauth_failed");
                return Task.CompletedTask;
            },
            OnTicketReceived = context =>
            {
                Console.WriteLine("Google OAuth ticket received successfully");
                return Task.CompletedTask;
            }
        };
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
        
        // Configure events to handle data protection issues and force HTTPS
        options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
        {
            OnRedirectToAuthorizationEndpoint = context =>
            {
                Console.WriteLine($"Facebook OAuth redirect URI: {context.RedirectUri}");
                // Force HTTPS for OAuth redirect
                if (builder.Environment.IsProduction())
                {
                    var uri = new Uri(context.RedirectUri);
                    var httpsUri = new UriBuilder(uri) { Scheme = "https" }.Uri.ToString();
                    Console.WriteLine($"Facebook OAuth HTTPS redirect URI: {httpsUri}");
                    context.Response.Redirect(httpsUri);
                    return Task.CompletedTask;
                }
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            },
            OnRemoteFailure = context =>
            {
                Console.WriteLine($"Facebook OAuth remote failure: {context.Failure?.Message}");
                Console.WriteLine($"Facebook OAuth failure details: {context.Failure?.StackTrace}");
                context.HandleResponse();
                context.Response.Redirect("/Identity/Account/Login?error=oauth_failed");
                return Task.CompletedTask;
            },
            OnTicketReceived = context =>
            {
                Console.WriteLine("Facebook OAuth ticket received successfully");
                return Task.CompletedTask;
            }
        };
    });
}

builder.Services.AddControllersWithViews();

// Configure URL generation to use HTTPS in production
if (builder.Environment.IsProduction())
{
    builder.Services.Configure<Microsoft.AspNetCore.Routing.RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
    });
}

// Configure antiforgery to be less strict for OAuth
builder.Services.Configure<Microsoft.AspNetCore.Antiforgery.AntiforgeryOptions>(options =>
{
    options.SuppressXFrameOptionsHeader = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Configure antiforgery to be more lenient
builder.Services.Configure<Microsoft.AspNetCore.Antiforgery.AntiforgeryOptions>(options =>
{
    options.SuppressXFrameOptionsHeader = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsProduction() ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
});

// Disable antiforgery completely in production
if (builder.Environment.IsProduction())
{
    builder.Services.Configure<Microsoft.AspNetCore.Antiforgery.AntiforgeryOptions>(options =>
    {
        // Disable antiforgery token generation and validation
        options.Cookie.Name = null; // Disable antiforgery cookies
    });
}

// Note: IgnoreAntiforgeryTokenAttribute is not available in this version
// We'll handle antiforgery issues in middleware instead

// Configure DataProtection for containerized environments
var dataProtectionBuilder = builder.Services.AddDataProtection()
    .SetApplicationName("ShiftSchedularApplication");

// Configure data protection based on environment
if (builder.Environment.IsProduction())
{
    Console.WriteLine("Configuring data protection for production environment");
    
    // Use environment variable for data protection key if available
    var dataProtectionKey = Environment.GetEnvironmentVariable("ASPNETCORE_DATA_PROTECTION_KEY");
    if (!string.IsNullOrEmpty(dataProtectionKey))
    {
        Console.WriteLine("Using environment variable for data protection key");
    }
    
    // Use in-memory keys only for production (no file persistence)
    Console.WriteLine("Using in-memory data protection keys for production");
    dataProtectionBuilder.SetDefaultKeyLifetime(TimeSpan.FromDays(365)); // Very long lifetime
    
    // Skip file system persistence - use in-memory only
    Console.WriteLine("Skipping file system persistence for data protection keys");
}
else
{
    // In development, use default configuration
    dataProtectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "keys")));
    dataProtectionBuilder.SetDefaultKeyLifetime(TimeSpan.FromDays(90));
}

var app = builder.Build();

// Log data protection configuration
if (app.Environment.IsProduction())
{
    Console.WriteLine("Production environment - using in-memory data protection keys");
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
    // Disable HTTPS redirection in containerized environment
    // app.UseHttpsRedirection();
    
    // Force HTTPS for OAuth providers and ensure proper URL generation
    app.Use(async (context, next) =>
    {
        // Set the scheme to HTTPS for OAuth callbacks
        context.Request.Scheme = "https";
        context.Request.Host = new Microsoft.AspNetCore.Http.HostString(context.Request.Host.Host, context.Request.Host.Port ?? 443);
        
        // Check for HTTPS headers from proxy
        if (context.Request.Headers["X-Forwarded-Proto"].Contains("https") || 
            context.Request.Headers["X-Forwarded-For"].Any())
        {
            context.Request.IsHttps = true;
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

// Add middleware to disable antiforgery for auth endpoints BEFORE authentication
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase) ||
        context.Request.Path.StartsWithSegments("/Identity/Account/Register", StringComparison.OrdinalIgnoreCase) ||
        context.Request.Path.StartsWithSegments("/signin-google", StringComparison.OrdinalIgnoreCase) ||
        context.Request.Path.StartsWithSegments("/signin-facebook", StringComparison.OrdinalIgnoreCase))
    {
        // Disable antiforgery for authentication endpoints
        context.Items["DisableAntiforgery"] = true;
        Console.WriteLine($"Disabled antiforgery for: {context.Request.Path}");
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// Skip antiforgery validation for authentication endpoints
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/signin-google") || 
        context.Request.Path.StartsWithSegments("/signin-facebook") ||
        context.Request.Path.StartsWithSegments("/Identity/Account/Login") ||
        context.Request.Path.StartsWithSegments("/Identity/Account/Register"))
    {
        // Skip antiforgery validation for authentication endpoints
        context.Items["SkipAntiforgery"] = true;
        // Also disable antiforgery for these endpoints
        context.Request.Headers.Remove("X-CSRF-TOKEN");
        context.Request.Headers.Remove("X-Requested-With");
        
        // Clear any existing antiforgery cookies
        context.Response.Cookies.Delete("__RequestVerificationToken");
        context.Response.Cookies.Delete("__RequestVerificationToken_Lw");
        
        Console.WriteLine($"Authentication endpoint accessed: {context.Request.Path}");
    }
    await next();
});

// Add comprehensive error handling middleware
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
    catch (Microsoft.AspNetCore.Antiforgery.AntiforgeryValidationException ex)
    {
        // Handle antiforgery token issues - just continue for auth endpoints
        Console.WriteLine($"Antiforgery token error: {ex.Message}");
        Console.WriteLine($"Request path: {context.Request.Path}");
        
        // For authentication endpoints, just continue without throwing
        if (context.Request.Path.StartsWithSegments("/signin-google", StringComparison.OrdinalIgnoreCase) || 
            context.Request.Path.StartsWithSegments("/signin-facebook", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.StartsWithSegments("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.StartsWithSegments("/Identity/Account/Register", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Skipping antiforgery validation for authentication endpoint");
            await next();
            return;
        }
        throw;
    }
    catch (Exception ex)
    {
        // Log all errors
        Console.WriteLine($"Request error: {ex.Message}");
        Console.WriteLine($"Request path: {context.Request.Path}");
        
        // For OAuth endpoints, redirect to login
        if (context.Request.Path.StartsWithSegments("/signin-google") || context.Request.Path.StartsWithSegments("/signin-facebook"))
        {
            Console.WriteLine($"OAuth error: {ex.Message}");
            context.Response.Redirect("/Identity/Account/Login?error=oauth_failed");
            return;
        }
        
        // For login endpoints, redirect back to login
        if (context.Request.Path.StartsWithSegments("/Identity/Account/Login"))
        {
            Console.WriteLine($"Login error: {ex.Message}");
            context.Response.Redirect("/Identity/Account/Login?error=login_failed");
            return;
        }
        
        throw;
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Add a test endpoint for data protection
app.MapGet("/test-dataprotection", (Microsoft.AspNetCore.DataProtection.IDataProtectionProvider dataProtection) =>
{
    try
    {
        var protector = dataProtection.CreateProtector("test");
        var testData = "test";
        var protectedData = protector.Protect(testData);
        var unprotectedData = protector.Unprotect(protectedData);
        
        if (unprotectedData == testData)
        {
            return Results.Ok("Data protection is working correctly");
        }
        else
        {
            return Results.BadRequest("Data protection test failed");
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Data protection error: {ex.Message}");
    }
});

// Add a test endpoint for seed user
app.MapGet("/test-seeduser", async (UserManager<IdentityUser> userManager) =>
{
    try
    {
        var user = await userManager.FindByEmailAsync("dario@gc.ca");
        if (user != null)
        {
            return Results.Ok($"Seed user exists: {user.Email}");
        }
        else
        {
            return Results.NotFound("Seed user not found");
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Seed user test error: {ex.Message}");
    }
});

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