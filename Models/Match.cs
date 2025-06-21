// Models/Match.cs
using Web_site_analytic_sports.Models;

public class Match
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Team1 { get; set; } = string.Empty;
    public string Team2 { get; set; } = string.Empty;
    public int Score1 { get; set; }
    public int Score2 { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;

    // Добавляем связь с турниром
    public int TournamentId { get; set; }
    public Tournament Tournament { get; set; } = null!; // Навигационное свойство
}