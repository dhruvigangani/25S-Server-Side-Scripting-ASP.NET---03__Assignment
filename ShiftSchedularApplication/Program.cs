using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularApplication.Data;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

var configConnection = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine($"Config Connection: {(string.IsNullOrEmpty(configConnection) ? "NOT SET" : "SET")}");

// Use SQLite connection string
var connectionString = configConnection ?? "Data Source=app.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; 
})
.AddEntityFrameworkStores<ApplicationDbContext>();

var authenticationBuilder = builder.Services.AddAuthentication();

// Only add Google authentication if ClientId is provided
var googleAuth = builder.Configuration.GetSection("Authentication:Google");
var clientId = googleAuth["ClientId"];
if (!string.IsNullOrEmpty(clientId))
{
    authenticationBuilder.AddGoogle(options =>
    {
        options.ClientId = clientId;
        options.ClientSecret = googleAuth["ClientSecret"] ?? string.Empty;
    });
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
builder.Services.AddDataProtection();

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    try
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        Console.WriteLine("Attempting to create database...");
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine("Database created successfully!");
        
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        await SeedUsersAsync(userManager);
        Console.WriteLine("User seeding completed!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during database initialization: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
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

// Configure HTTPS redirection
app.UseHttpsRedirection();
app.UseStaticFiles();

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
            var result = await userManager.CreateAsync(newUser, "Test123$");
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
            Console.WriteLine("User 'dario@gc.ca' already exists!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during user seeding: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        // Don't throw - user seeding failure shouldn't stop the app
    }
}