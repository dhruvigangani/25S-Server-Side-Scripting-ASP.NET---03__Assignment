using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularApplication.Data;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var configConnection = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine($"DATABASE_URL: {(string.IsNullOrEmpty(databaseUrl) ? "NOT SET" : "SET")}");
Console.WriteLine($"Config Connection: {(string.IsNullOrEmpty(configConnection) ? "NOT SET" : "SET")}");

// Convert PostgreSQL URL to connection string if needed
var connectionString = databaseUrl;
if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgresql://"))
{
    try
    {
        // Parse the PostgreSQL URL format
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432; // Default to 5432 if port is not specified
        var database = uri.AbsolutePath.TrimStart('/');
        
        // Ensure database name is not empty
        if (string.IsNullOrEmpty(database))
        {
            database = "shiftschedulerdb";
        }
        
        connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
        
        Console.WriteLine($"Converted PostgreSQL URL to connection string format");
        Console.WriteLine($"Host: {host}, Port: {port}, Database: {database}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error parsing DATABASE_URL: {ex.Message}");
        Console.WriteLine($"DATABASE_URL value: {databaseUrl}");
        throw;
    }
}

connectionString = connectionString
    ?? configConnection
    ?? throw new InvalidOperationException("DATABASE_URL environment variable not found. Please set it in Render environment variables.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; 
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var googleAuth = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = googleAuth["ClientId"] ?? string.Empty;
        options.ClientSecret = googleAuth["ClientSecret"] ?? string.Empty;
    });

builder.Services.AddControllersWithViews();

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
        throw;
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
        // Don't throw - user seeding failure shouldn't stop the app
    }
}