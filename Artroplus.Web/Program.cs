using Artroplus.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// API İSTEMCİLERİ (CLAUDE.md Mimari Kural)
// Web katmanı DB'ye doğrudan erişemez; tüm veri işlemleri Artroplus.Api üzerinden yapılır.
// =============================================
var apiBaseUrl = builder.Configuration["ArtroplusApi:BaseUrl"] ?? "https://localhost:7000";

builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IGuncellemeApiService, GuncellemeApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

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
