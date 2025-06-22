using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Web_site_analytic_sports.Data;
using Web_site_analytic_sports.Models;

namespace Web_site_analytic_sports.Services
{
    public class DataService : IDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DataService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task SeedWorldCupDataAsync(int year)
        {
            if (await _context.Tournaments.AnyAsync(t => t.Year == year))
                return;

            var tournament = new Tournament
            {
                Year = year,
                Name = $"World Cup {year}",
                Country = GetHostCountry(year)
            };

            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync();

            string folderName = GetHostFolderName(year);
            string dataPath = Path.Combine(_env.WebRootPath, "data", "worldcup", $"{year}--{folderName}");

            if (!Directory.Exists(dataPath))
                return;

            await ParseGroupMatches(dataPath, tournament.Id, year);
        }

        private string GetHostFolderName(int year)
        {
            return year switch
            {
                1930 => "uruguay",
                1934 => "italy",
                1938 => "france",
                1950 => "brazil",
                1954 => "switzerland",
                1958 => "sweden",
                1962 => "chile",
                1966 => "england",
                1998 => "france",
                2002 => "south-korea-japan",
                2006 => "germany",
                2010 => "south-africa",
                2014 => "brazil",
                2018 => "russia",
                2022 => "qatar",
                _ => "unknown"
            };
        }

        private async Task ParseGroupMatches(string dataPath, int tournamentId, int year)
        {
            string groupFile = Path.Combine(dataPath, "group.md");

            if (!File.Exists(groupFile))
                return;

            string[] lines = await File.ReadAllLinesAsync(groupFile);
            string? currentGroup = null;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("Group "))
                {
                    currentGroup = line.Substring(6, 1);
                    continue;
                }

                var matchObj = ParseMatchLine(line, year);

                if (matchObj != null)
                {
                    matchObj.Group = currentGroup;
                    matchObj.TournamentId = tournamentId;
                    matchObj.Stage = "Group";
                    _context.FootballMatches.Add(matchObj);
                }
            }

            await _context.SaveChangesAsync();
        }

        private FootballMatch? ParseMatchLine(string line, int year)
        {
            var regex = new Regex(@"\((\d+)\)\s+(\d+)\s+(\w+)\s+([\w\s]+?)\s+(\d+)-(\d+)\s+([\w\s]+?)\s+@\s+([\w\s,]+)");
            var regexMatch = regex.Match(line);

            if (!regexMatch.Success)
                return null;

            try
            {
                int day = int.Parse(regexMatch.Groups[2].Value);
                string monthName = regexMatch.Groups[3].Value;
                string team1 = regexMatch.Groups[4].Value.Trim();
                int score1 = int.Parse(regexMatch.Groups[5].Value);
                int score2 = int.Parse(regexMatch.Groups[6].Value);
                string team2 = regexMatch.Groups[7].Value.Trim();
                string location = regexMatch.Groups[8].Value.Trim();

                int month = DateTime.ParseExact(monthName, "MMMM", CultureInfo.InvariantCulture).Month;
                var date = new DateTime(year, month, day);

                return new FootballMatch
                {
                    Date = date,
                    Team1 = team1,
                    Team2 = team2,
                    Score1 = score1,
                    Score2 = score2,
                    Location = location
                };
            }
            catch
            {
                return null;
            }
        }

        private string GetHostCountry(int year)
        {
            return year switch
            {
                1930 => "Uruguay",
                1934 => "Italy",
                1938 => "France",
                1950 => "Brazil",
                1954 => "Switzerland",
                1958 => "Sweden",
                1962 => "Chile",
                1966 => "England",
                1998 => "France",
                2002 => "South Korea & Japan",
                2006 => "Germany",
                2010 => "South Africa",
                2014 => "Brazil",
                2018 => "Russia",
                2022 => "Qatar",
                _ => "Unknown"
            };
        }
    }
}