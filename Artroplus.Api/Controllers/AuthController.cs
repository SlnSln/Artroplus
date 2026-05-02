using Artroplus.Core.DTOs;
using Artroplus.Core.Entities;
using Artroplus.Core.IInterface;
using Artroplus.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artroplus.Api.Controllers;

/// <summary>
/// Kimlik doğrulama API endpoint'leri.
/// CLAUDE.md Kural 3: Bu endpoint localhost'tan çağrılır (IP whitelist middleware aktif).
/// </summary>
public class AuthController : BaseApiController
{
    private readonly IGenericService<Kullanici> _kullaniciService;
    private readonly IGenericService<Rol> _rolService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IGenericService<Kullanici> kullaniciService,
        IGenericService<Rol> rolService,
        ILogger<AuthController> logger)
    {
        _kullaniciService = kullaniciService;
        _rolService = rolService;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcı adı ve şifre ile giriş yapar, kullanıcı bilgilerini döner.
    /// </summary>
    // [AllowAnonymous] Gerekçe: Login endpoint'i kimlik doğrulama gerektirmez
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.KullaniciAdi) || string.IsNullOrWhiteSpace(request.Sifre))
            return Fail<LoginResponseDto>("Kullanıcı adı ve şifre gereklidir.");

        var users = await _kullaniciService.FindAsync(x => x.KullaniciAdi == request.KullaniciAdi);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Sifre, user.SifreHash))
        {
            _logger.LogWarning("Başarısız giriş denemesi: {KullaniciAdi}", request.KullaniciAdi);
            return Fail<LoginResponseDto>("Kullanıcı adı veya şifre hatalı.");
        }

        var rol = await _rolService.GetByIdAsync(user.RolId);

        var response = new LoginResponseDto
        {
            Id = user.Id,
            Ad = user.Ad,
            Soyad = user.Soyad,
            RolAd = rol?.Ad ?? "Kullanıcı"
        };

        _logger.LogInformation("Kullanıcı giriş yaptı: {KullaniciAdi}", request.KullaniciAdi);
        return Success(response, "Giriş başarılı.");
    }
}
