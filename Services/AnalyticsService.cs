using System.Text.RegularExpressions;
using Web_site_analytic_sports.Models;

namespace Web_site_analytic_sports.Services;

public class AnalyticsService : IAnalyticsService
{
    public Task<AnalysisResult> AnalyzeMatchAsync(Match match)
    {
        // Заглушка - реализуйте настоящую логику позже
        return Task.FromResult(new AnalysisResult());
    }
}

public interface IAnalyticsService
{
    Task<AnalysisResult> AnalyzeMatchAsync(Match match);
}

public class AnalysisResult
{
    // Добавьте необходимые свойства
    public double WinProbability { get; set; }
}