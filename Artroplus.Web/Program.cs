using Artroplus.Core.IInterface;
using Artroplus.Core.IRepositories;
using Artroplus.Data.Context;
using Artroplus.Data.Repositories;
using Artroplus.Service.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// BAĞLANTI AYARLARI
// CLAUDE.md Kural 4: Connection string Environment Variable'dan okunur
// =============================================
var connectionString = Environment.GetEnvironmentVariable("ARTROPLUS_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ArtroplusDbContext>(options =>
    options.UseSqlServer(connectionString));

// =============================================
// DI KAYITLARI
// CLAUDE.md Kural 7: Servisler interface üzerinden kaydedilir
// =============================================
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

// =============================================
// AUTHENTICATION & AUTHORIZATION
// CLAUDE.md Kural 1: Tüm sayfalar korunmalıdır
// =============================================
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// MVC + Antiforgery (CLAUDE.md Kural 2)
builder.Services.AddControllersWithViews(options =>
{
    // Global antiforgery filter — tüm POST isteklerinde CSRF koruması
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
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

app.Run();
