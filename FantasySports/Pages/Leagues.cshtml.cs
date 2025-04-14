using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FantasySports.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace FantasySports.Pages
{
    public class LeaguesModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<League> Leagues { get; set; } = new();
        public List<Team> AvailableTeams { get; set; } = new();
        [BindProperty] public string? ErrorMessage { get; set; }

        public LeaguesModel(HttpClient httpClient) => _httpClient = httpClient;

        public async Task OnGetAsync()
        {
            try
            {
                Leagues = await _httpClient.GetFromJsonAsync<List<League>>("https://localhost:5001/api/leagues")
                         ?? new List<League>();
                AvailableTeams = await _httpClient.GetFromJsonAsync<List<Team>>("https://localhost:5001/api/teams")
                               ?? new List<Team>();
            }
            catch
            {
                ErrorMessage = "Failed to load leagues";
            }
        }

        public async Task<List<Team>> GetAvailableTeams()
        {
            if (!AvailableTeams.Any())
            {
                AvailableTeams = await _httpClient.GetFromJsonAsync<List<Team>>("https://localhost:5001/api/teams")
                               ?? new List<Team>();
            }
            return AvailableTeams;
        }

        public async Task<IActionResult> OnPostCreateLeague(string leagueName)
        {
            if (string.IsNullOrWhiteSpace(leagueName))
            {
                ErrorMessage = "League name is required";
                return Page();
            }

            await _httpClient.PostAsJsonAsync("https://localhost:5001/api/leagues",
                new League { Name = leagueName });

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostJoinLeague(int leagueId, int teamId)
        {
            await _httpClient.PostAsJsonAsync($"https://localhost:5001/api/leagues/{leagueId}/join",
                new { teamId });

            return RedirectToPage();
        }
    }
}