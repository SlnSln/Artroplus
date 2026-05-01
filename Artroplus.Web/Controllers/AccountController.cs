using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Artroplus.Web.Controllers;

/// <summary>
/// CLAUDE.md Kural 1: [AllowAnonymous] yalnızca Login/Register için kullanılabilir.
/// Bu controller'ın tüm action'ları gerekçesiyle AllowAnonymous içerir.
/// </summary>
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    // [AllowAnonymous] Gerekçe: Login sayfası kimlik doğrulama gerektirmez
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // [AllowAnonymous] Gerekçe: Login POST işlemi kimlik doğrulama gerektirmez
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken] // CLAUDE.md Kural 2: CSRF koruması zorunlu
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        // TODO: Gerçek authentication servisi ile değiştirilecek
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Kullanıcı adı ve şifre gereklidir.");
            return View();
        }

        // Örnek claims — gerçek implementasyonda veritabanından okunacak
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "User")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        _logger.LogInformation("Kullanıcı giriş yaptı: {Username}", username);

        return LocalRedirect(returnUrl ?? "/");
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // CLAUDE.md Kural 2: CSRF koruması zorunlu
    public async Task<IActionResult> Logout()
    {
        var username = User.Identity?.Name;
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("Kullanıcı çıkış yaptı: {Username}", username);
        return RedirectToAction("Login");
    }

    // [AllowAnonymous] Gerekçe: Erişim engellendi sayfası herkese açık olmalı
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
