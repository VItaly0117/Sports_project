using System.Threading.Tasks;

namespace Web_site_analytic_sports.Services
{
    public interface IAnalyticsService
    {
        Task<PredictionResult> PredictMatchAsync(string team1, string team2);
    }

    public class PredictionResult
    {
        public double Team1WinProbability { get; set; }
        public double Team2WinProbability { get; set; }
        public double DrawProbability { get; set; }
        public required string PredictedScore { get; set; }
    }
}