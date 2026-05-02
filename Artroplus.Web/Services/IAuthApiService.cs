using Artroplus.Core.DTOs;

namespace Artroplus.Web.Services;

/// <summary>
/// Artroplus.Api üzerinden kimlik doğrulama işlemlerini gerçekleştiren servis arayüzü.
/// </summary>
public interface IAuthApiService
{
    /// <summary>
    /// API'ye HTTP POST isteği göndererek kimlik doğrular.
    /// Başarılı girişte kullanıcı bilgilerini, başarısızda null döner.
    /// </summary>
    Task<LoginResponseDto?> LoginAsync(string kullaniciAdi, string sifre);
}
