using System.ComponentModel.DataAnnotations;

namespace Web_site_analytic_sports.Models
{
    public class FootballMatch
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100)]
        public string Team1 { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Team2 { get; set; } = string.Empty;

        public int? Score1 { get; set; }
        public int? Score2 { get; set; }

        [StringLength(50)]
        public string? Group { get; set; }

        [StringLength(50)]
        public string Stage { get; set; } = "Group";

        [StringLength(200)]
        public string? Location { get; set; }

        public int TournamentId { get; set; }
        public Tournament? Tournament { get; set; }
    }
}