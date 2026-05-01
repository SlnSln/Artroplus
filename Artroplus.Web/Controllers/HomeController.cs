using Artroplus.Core.Entities;
using Artroplus.Core.IInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Artroplus.Web.Controllers;

/// <summary>
/// CLAUDE.md Kural 1: Tüm controller'lar [Authorize] ile korunmalıdır.
/// </summary>
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IGenericService<GuncellemeNotu> _guncellemeService;

    public HomeController(ILogger<HomeController> logger, IGenericService<GuncellemeNotu> guncellemeService)
    {
        _logger = logger;
        _guncellemeService = guncellemeService;
    }

    public async Task<IActionResult> Index()
    {
        var notlar = await _guncellemeService.GetAllAsync();
        var sonNotlar = notlar.OrderByDescending(x => x.OlusturulmaTarihi).Take(5).ToList();
        
        return View(sonNotlar);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
