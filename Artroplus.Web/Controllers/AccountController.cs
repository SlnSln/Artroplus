using Artroplus.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Artroplus.Web.Controllers;

/// <summary>
/// CLAUDE.md Kural 1: [AllowAnonymous] yalnızca Login/Register için kullanılabilir.
/// CLAUDE.md Mimari Kural: Kimlik doğrulama Artroplus.Api üzerinden gerçekleştirilir.
/// </summary>
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAuthApiService _authApiService;

    public AccountController(ILogger<AccountController> logger, IAuthApiService authApiService)
    {
        _logger = logger;
        _authApiService = authApiService;
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

        // Kimlik doğrulama Artroplus.Api üzerinden yapılır (CLAUDE.md Mimari Kural)
        var kullanici = await _authApiService.LoginAsync(username, password);

        if (kullanici == null)
        {
            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, kullanici.Ad + " " + kullanici.Soyad),
            new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
            new Claim(ClaimTypes.Role, kullanici.RolAd)
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
