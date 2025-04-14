using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FantasySports.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace FantasySports.Pages
{
    public class TeamsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<Team> Teams { get; set; } = new();
        [BindProperty] public string? ErrorMessage { get; set; }

        public TeamsModel(HttpClient httpClient) => _httpClient = httpClient;

        public async Task OnGetAsync()
        {
            try
            {
                Teams = await _httpClient.GetFromJsonAsync<List<Team>>("https://localhost:5001/api/teams")
                       ?? new List<Team>();
            }
            catch
            {
                ErrorMessage = "Failed to load teams";
            }
        }

        public async Task<IActionResult> OnPostCreateTeam(string teamName)
        {
            if (string.IsNullOrWhiteSpace(teamName))
            {
                ErrorMessage = "Team name is required";
                return Page();
            }

            await _httpClient.PostAsJsonAsync("https://localhost:5001/api/teams",
                new Team { Name = teamName });

            return RedirectToPage();
        }
    }
}