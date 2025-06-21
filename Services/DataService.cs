using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        public interface IDataService
        {
            Task SeedWorldCupDataAsync(int year);
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

            string dataPath = Path.Combine(_env.ContentRootPath, "Data", "worldcup", $"{year}--{tournament.Country.ToLower()}");

            if (!Directory.Exists(dataPath))
                return;

            await ParseGroupMatches(dataPath, tournament.Id);
        }

        private async Task ParseGroupMatches(string dataPath, int tournamentId)
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

                var MatchMode = ParseMatchLine(line);
                if (MatchMode != null)
                {
                    MatchMode.Group = currentGroup;
                    MatchMode.TournamentId = tournamentId;
                    _context.Matches.Add(MatchMode);
                }
            }

            await _context.SaveChangesAsync();
        }

        private MatchMode? ParseMatchLine(string line)
        {
            var regex = new Regex(@"\((\d+)\)\s+(\d+)\s+(\w+)\s+([\w\s]+?)\s+(\d+)-(\d+)\s+([\w\s]+?)\s+@\s+([\w\s,]+)");
            var match = regex.Match(line);

            if (!match.Success)
                return null;

            try
            {
                int day = int.Parse(match.Groups[2].Value);
                string monthName = match.Groups[3].Value;
                string team1 = match.Groups[4].Value.Trim();
                int score1 = int.Parse(match.Groups[5].Value);
                int score2 = int.Parse(match.Groups[6].Value);
                string team2 = match.Groups[7].Value.Trim();
                string location = match.Groups[8].Value.Trim();

                int month = DateTime.ParseExact(monthName, "MMMM", CultureInfo.InvariantCulture).Month;
                var date = new DateTime(1998, month, day);

                return new MatchMode
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