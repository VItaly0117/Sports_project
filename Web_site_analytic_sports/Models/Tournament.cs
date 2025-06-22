namespace Web_site_analytic_sports.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public List<FootballMatch> Matches { get; set; } = new List<FootballMatch>();
    }
}