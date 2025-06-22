using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_site_analytic_sports.Data;
using Web_site_analytic_sports.Models;

namespace Web_site_analytic_sports.Controllers
{
    public class MatchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int year = 1998)
        {
            var tournament = await _context.Tournaments
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Year == year);

            if (tournament == null)
                return NotFound();

            var matches = await _context.FootballMatches
                .Include(m => m.Tournament)
                .Where(m => m.TournamentId == tournament.Id)
                .OrderBy(m => m.Date)
                .ToListAsync();

            ViewBag.Tournament = tournament;
            ViewBag.Years = await _context.Tournaments
                .Select(t => t.Year)
                .Distinct()
                .ToListAsync();

            return View(matches);
        }

        public async Task<IActionResult> Details(int id)
        {
            var match = await _context.FootballMatches
                .Include(m => m.Tournament)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound();

            return View(match);
        }
    }
}