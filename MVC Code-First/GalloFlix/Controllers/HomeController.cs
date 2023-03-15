using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GalloFlix.Models;
using GalloFlix.Data;
using Microsoft.EntityFrameworkCore;

namespace GalloFlix.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        Random rand = new Random();
        var movies = await _context.Movies.OrderBy(r => EF.Functions.Random())
            .Include(m => m.Genres).ThenInclude(mr => mr.Genre)
            .Include(m => m.Ratings)
            .Take(8).ToListAsync();
        return View(movies);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
