using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GalloFlix.Models;
using GalloFlix.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

    public async Task<IActionResult> AllMovies(string title)
    {
        if (title == null || title == string.Empty)
        {
            var movies = await _context.Movies.OrderByDescending(m => m.Ratings.Sum(mv => mv.RatingValue))
                .Include(m => m.Genres).ThenInclude(mr => mr.Genre)
                .Include(m => m.Ratings)
                .ToListAsync();
            return View(movies);
        }
        else
        {
            var movies = await _context.Movies.OrderByDescending(m => m.Ratings.Sum(mv => mv.RatingValue))
                .Where(m => m.Title.Contains(title))
                .Include(m => m.Genres).ThenInclude(mr => mr.Genre)
                .Include(m => m.Ratings)
                .ToListAsync();
            return View(movies);
        }
    }

    public async Task<IActionResult> Detail(int id)
    {
        var movie = await _context.Movies
            .Include(m => m.Genres).ThenInclude(mr => mr.Genre)
            .Include(m => m.Ratings)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
            return PageNotFound();
        return View(movie);
    }

    public async Task<IActionResult> Rate(int Id, byte rating)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var movieRating = _context.MovieRatings.Where(mr => mr.MovieId == Id && mr.UserId.Equals(userId)).FirstOrDefault();
        if (movieRating == null)
        {
            movieRating = new MovieRating()
            {
                MovieId = Id,
                UserId = userId,
                RatingValue = rating
            };
            _context.MovieRatings.Add(movieRating);
        }
        else
        {
            movieRating.RatingValue = rating;
            movieRating.RatingDate = DateTime.Now;
            _context.MovieRatings.Update(movieRating);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Detail), new { Id = Id });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult TermsOfUse()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult PageNotFound()
    {
        return View();
    }
}
