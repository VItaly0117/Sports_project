using System.ComponentModel.DataAnnotations;

namespace Web_site_analytic_sports.Models
{
    public class Tournament
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        public int Year { get; set; }

        public List<MatchMode> Matches { get; set; } = new List<MatchMode>();
    }
}