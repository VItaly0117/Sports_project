using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_site_analytic_sports.Data;
using Web_site_analytic_sports.Models;
using Web_site_analytic_sports.Services;

namespace Web_site_analytic_sports.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ApplicationDbContext _context;

        public AnalyticsController(IAnalyticsService analyticsService, ApplicationDbContext context)
        {
            _analyticsService = analyticsService;
            _context = context;
        }

        public async Task<IActionResult> Predict(int matchId)
        {
            var match = await _context.FootballMatches.FindAsync(matchId);
            if (match == null) return NotFound();

            var prediction = await _analyticsService.PredictMatchAsync(match.Team1, match.Team2);

            var viewModel = new PredictionViewModel
            {
                Match = match,
                Prediction = prediction
            };

            return View(viewModel);
        }
    }

    public class PredictionViewModel
    {
        public required FootballMatch Match { get; set; }
        public required PredictionResult Prediction { get; set; }
    }
}