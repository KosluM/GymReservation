using GymReservation.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsý
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity (kullanýcý + rol sistemi)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Mail onayý isteme
})
    .AddRoles<IdentityRole>()                          // Rol desteði (Admin vs.)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// -------------------------------------------------------
// ADMIN ROLÜ VE ADMIN KULLANICI OLUÞTURMA (SEEDING)
// -------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // 1) Admin rolünü oluþtur (yoksa)
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // 2) Admin kullanýcýsýný oluþtur (yoksa)
    string adminEmail = "admin@sau.com";
    string adminPassword = "Admin123*";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        // istersen result.Succeeded kontrolü ekleyebiliriz ama þimdilik þart deðil
    }

    // 3) Kullanýcýyý Admin rolüne ekle (ekli deðilse)
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// HTTP pipeline
if (app.Environment.IsDevelopment())
{
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
