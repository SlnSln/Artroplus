using Artroplus.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artroplus.Web.Controllers;

/// <summary>
/// CLAUDE.md Kural 1: Tüm controller'lar [Authorize] ile korunmalıdır.
/// CLAUDE.md Mimari Kural: Veri erişimi Artroplus.Api üzerinden gerçekleştirilir.
/// </summary>
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IGuncellemeApiService _guncellemeApiService;

    public HomeController(ILogger<HomeController> logger, IGuncellemeApiService guncellemeApiService)
    {
        _logger = logger;
        _guncellemeApiService = guncellemeApiService;
    }

    public async Task<IActionResult> Index()
    {
        var sonNotlar = await _guncellemeApiService.GetSonNotlarAsync(5);
        return View(sonNotlar);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
