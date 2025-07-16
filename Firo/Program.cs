using Firo.Common.Services;
using Firo.Infrastructure.Data;
using Firo.Web.Seeders;
using Firo.Web.Service;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
IConfiguration Configuration = builder.Configuration;

// Check and set connection string BEFORE building the application
var connFilePath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "Conn", "Connection.txt");
string connectionString = string.Empty;

if (File.Exists(connFilePath))
{
    connectionString = File.ReadAllText(connFilePath).Trim();
}

// Set the connection string in configuration
builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register ConnectionCheck service
builder.Services.AddScoped<ConnectionCheck>();
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

// Register DbContext with the validated connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Firo.Infrastructure")));


builder.Services.AddMemoryCache();

// Register Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();


// Register repositories
builder.Services.AddRepositories();
builder.Services.AddSingleton<FileUploadService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    if (string.IsNullOrEmpty(env.WebRootPath))
    {
        throw new InvalidOperationException("WebRootPath is not set.");
    }
    return new FileUploadService(env.WebRootPath);
});


builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(365 * 100); // 100 years (effectively lifetime)
    options.Cookie.HttpOnly = true;  // Prevents JavaScript access
    options.Cookie.IsEssential = true;  // Ensures session persists even if cookies require consent
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();




// Middleware to ensure connection string is valid
app.Use(async (context, next) =>
{
    // Skip connection check for requests to /Startup/Index
    if (context.Request.Path.StartsWithSegments("/Startup/Index") ||
        context.Request.Path.StartsWithSegments("/Startup/SaveConnectionString"))
    {
        await next();
        return;
    }

    var connCheck = context.RequestServices.GetRequiredService<ConnectionCheck>();
    if (!connCheck.HasValidConnectionString())
    {
        context.Response.Redirect("/Startup/Index");
        return;
    }

    await next();
});

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await RoleSeeder.Seed(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the roles.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseDeveloperExceptionPage();
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();


// Route for areas with fallback to 'Home/Index'
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default route for non-areas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();





// ConnectionCheck class to validate the connection string
public class ConnectionCheck
{
    private readonly IConfiguration _configuration;

    public ConnectionCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool HasValidConnectionString()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        return !string.IsNullOrEmpty(connectionString);
    }
}
