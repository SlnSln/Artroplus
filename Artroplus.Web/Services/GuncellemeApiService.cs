using System.Net.Http.Json;
using Artroplus.Core.DTOs;
using Artroplus.Core.Models;

namespace Artroplus.Web.Services;

/// <summary>
/// CLAUDE.md Mimari Kural: Web katmanı DB'ye doğrudan erişemez.
/// Güncelleme notları Artroplus.Api üzerinden HTTP ile getirilir.
/// </summary>
public class GuncellemeApiService : IGuncellemeApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GuncellemeApiService> _logger;

    public GuncellemeApiService(HttpClient httpClient, ILogger<GuncellemeApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<GuncellemeNotuDto>> GetSonNotlarAsync(int adet = 5)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<GuncellemeNotuDto>>>(
                $"api/guncellemenotlari/son/{adet}");

            return result?.Data ?? Enumerable.Empty<GuncellemeNotuDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GuncellemeNotlari API çağrısında hata oluştu");
            return Enumerable.Empty<GuncellemeNotuDto>();
        }
    }
}
