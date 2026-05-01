using Artroplus.Core.Entities;
using Artroplus.Core.IInterface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BCrypt.Net;

namespace Artroplus.Web.Controllers;

/// <summary>
/// CLAUDE.md Kural 1: [AllowAnonymous] yalnızca Login/Register için kullanılabilir.
/// Bu controller'ın tüm action'ları gerekçesiyle AllowAnonymous içerir.
/// </summary>
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IGenericService<Kullanici> _kullaniciService;
    private readonly IGenericService<Rol> _rolService;

    public AccountController(ILogger<AccountController> logger, IGenericService<Kullanici> kullaniciService, IGenericService<Rol> rolService)
    {
        _logger = logger;
        _kullaniciService = kullaniciService;
        _rolService = rolService;
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
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Kullanıcı adı ve şifre gereklidir.");
            return View();
        }

        // Veritabanından kullanıcıyı bul (IsDeleted = false otomatik uygulanır)
        var users = await _kullaniciService.FindAsync(x => x.KullaniciAdi == username);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.SifreHash))
        {
            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            return View();
        }

        // Kullanıcının rolünü al
        var rol = await _rolService.GetByIdAsync(user.RolId);
        string rolAd = rol?.Ad ?? "User";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Ad + " " + user.Soyad),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, rolAd)
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

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);
            
        return RedirectToAction("Index", "Home");
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
