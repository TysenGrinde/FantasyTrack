using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FantasySports.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace FantasySports.Pages
{
    public class DraftModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public Team? CurrentTeam { get; set; }
        public List<Athlete> AvailableAthletes { get; set; } = new();
        [BindProperty] public string? ErrorMessage { get; set; }

        public DraftModel(HttpClient httpClient) => _httpClient = httpClient;

        public async Task OnGetAsync(int teamId)
        {
            try
            {
                CurrentTeam = await _httpClient.GetFromJsonAsync<Team>($"https://localhost:5001/api/teams/{teamId}");
                AvailableAthletes = await _httpClient.GetFromJsonAsync<List<Athlete>>("https://localhost:5001/api/athletes")
                                   ?? new List<Athlete>();
            }
            catch
            {
                ErrorMessage = "Failed to load draft data";
            }
        }

        public async Task<IActionResult> OnPostMakePick(int teamId, int athleteId)
        {
            await _httpClient.PostAsJsonAsync("https://localhost:5001/api/draft/pick",
                new { leagueId = 1, teamId, athleteId });

            return RedirectToPage(new { teamId });
        }
    }
}