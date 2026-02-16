using CorporateHelpdesk.Data;
using CorporateHelpdesk.Interfaces;
using CorporateHelpdesk.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ITicketService, TicketService>(); //rejestracja service

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// --- POCZ�TEK SEKCJI DOCKERA ---
// To sprawi, �e aplikacja sama stworzy baz� danych przy starcie kontenera
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Pobieramy Tw�j kontekst bazy
        var context = services.GetRequiredService<CorporateHelpdesk.Data.ApplicationDbContext>();

        // Magiczna komenda: je�li bazy nie ma -> stw�rz j�. Je�li s� zmiany -> zaktualizuj.
        context.Database.Migrate();
        Console.WriteLine("--> Migracja bazy danych zako�czona sukcesem!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> B��d migracji: {ex.Message}");
    }
}

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedRolesAndAdminAsync(scope.ServiceProvider);
}

app.Run();
