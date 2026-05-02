using Artroplus.Core.DTOs;

namespace Artroplus.Web.Services;

/// <summary>
/// Artroplus.Api üzerinden güncelleme notlarını getiren servis arayüzü.
/// </summary>
public interface IGuncellemeApiService
{
    Task<IEnumerable<GuncellemeNotuDto>> GetSonNotlarAsync(int adet = 5);
}
