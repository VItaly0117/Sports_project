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

        public async Task<IActionResult> Index(int year = 1998, string stage = "Group")
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Matches)
                .FirstOrDefaultAsync(t => t.Year == year);

            if (tournament == null)
                return NotFound();

            var matches = tournament.Matches
                .Where(m => m.Stage == stage)
                .OrderBy(m => m.Date)
                .ToList();

            ViewBag.Tournament = tournament;
            ViewBag.Stage = stage;
            ViewBag.Years = await _context.Tournaments.Select(t => t.Year).Distinct().ToListAsync();

            return View(matches);
        }

        public async Task<IActionResult> Details(int id)
        {
            // Используйте Match вместо matchObj
            var match = await _context.Matches
                .Include(m => m.Tournament) // Теперь будет работать
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound();

            return View(match);
        }
    }
}