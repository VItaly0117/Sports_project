using Microsoft.EntityFrameworkCore;
using Web_site_analytic_sports.Data;
using Web_site_analytic_sports.Models;

namespace Web_site_analytic_sports.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PredictionResult> PredictMatchAsync(string team1, string team2)
        {
            var team1Matches = await _context.FootballMatches
                .Where(m => m.Team1 == team1 || m.Team2 == team1)
                .ToListAsync();

            var team2Matches = await _context.FootballMatches
                .Where(m => m.Team1 == team2 || m.Team2 == team2)
                .ToListAsync();

            double team1WinRate = team1Matches
                .Where(m => (m.Team1 == team1 && m.Score1 > m.Score2) ||
                           (m.Team2 == team1 && m.Score2 > m.Score1))
                .Count() / (double)team1Matches.Count;

            double team2WinRate = team2Matches
                .Where(m => (m.Team1 == team2 && m.Score1 > m.Score2) ||
                           (m.Team2 == team2 && m.Score2 > m.Score1))
                .Count() / (double)team2Matches.Count;

            double drawProbability = 1 - (team1WinRate + team2WinRate) / 2;

            return new PredictionResult
            {
                Team1WinProbability = team1WinRate,
                Team2WinProbability = team2WinRate,
                DrawProbability = drawProbability,
                PredictedScore = "2-1"
            };
        }
    }
}