using Artroplus.Core.DTOs;
using Artroplus.Core.Entities;
using Artroplus.Core.IInterface;
using Artroplus.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artroplus.Api.Controllers;

/// <summary>
/// Güncelleme notları API endpoint'leri.
/// CLAUDE.md Kural 3: Bu endpoint'ler yalnızca localhost'tan erişilebilir (IP whitelist aktif).
/// </summary>
public class GuncellemeNotlariController : BaseApiController
{
    private readonly IGenericService<GuncellemeNotu> _guncellemeService;

    public GuncellemeNotlariController(IGenericService<GuncellemeNotu> guncellemeService)
    {
        _guncellemeService = guncellemeService;
    }

    /// <summary>
    /// En son güncelleme notlarını döner.
    /// </summary>
    // [AllowAnonymous] Gerekçe: Server-to-server çağrı; IP whitelist dış erişimi zaten engeller
    [AllowAnonymous]
    [HttpGet("son/{adet:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<GuncellemeNotuDto>>>> GetSon(int adet = 5)
    {
        var notlar = await _guncellemeService.GetAllAsync();

        var dto = notlar
            .OrderByDescending(x => x.OlusturulmaTarihi)
            .Take(adet)
            .Select(x => new GuncellemeNotuDto
            {
                Id = x.Id,
                Baslik = x.Baslik,
                Icerik = x.Icerik,
                Versiyon = x.Versiyon,
                Tip = x.Tip,
                OlusturulmaTarihi = x.OlusturulmaTarihi
            });

        return Success<IEnumerable<GuncellemeNotuDto>>(dto);
    }
}
