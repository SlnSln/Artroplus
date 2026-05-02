using System.Net.Http.Json;
using Artroplus.Core.DTOs;
using Artroplus.Core.Models;

namespace Artroplus.Web.Services;

/// <summary>
/// CLAUDE.md Mimari Kural: Web katmanı DB'ye doğrudan erişemez.
/// Login doğrulaması Artroplus.Api üzerinden HTTP ile gerçekleştirilir.
/// </summary>
public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthApiService> _logger;

    public AuthApiService(HttpClient httpClient, ILogger<AuthApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<LoginResponseDto?> LoginAsync(string kullaniciAdi, string sifre)
    {
        try
        {
            var request = new LoginRequestDto { KullaniciAdi = kullaniciAdi, Sifre = sifre };
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();

            if (result?.Success == true)
                return result.Data;

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auth API çağrısında beklenmeyen hata oluştu");
            return null;
        }
    }
}
