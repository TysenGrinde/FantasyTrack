using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FantasySports.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace FantasySports.Pages
{
    public class AthleteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<Athlete> Athletes { get; set; } = new();
        [BindProperty] public string? ErrorMessage { get; set; }

        public AthleteModel(HttpClient httpClient) => _httpClient = httpClient;

        public async Task OnGetAsync()
        {
            try
            {
                Athletes = await _httpClient.GetFromJsonAsync<List<Athlete>>("https://localhost:5001/api/athletes")
                          ?? new List<Athlete>();
            }
            catch
            {
                ErrorMessage = "Failed to load athletes";
            }
        }

        public async Task<IActionResult> OnPostAddAthlete(string name, string sport)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(sport))
            {
                ErrorMessage = "Name and sport are required";
                return Page();
            }

            await _httpClient.PostAsJsonAsync("https://localhost:5001/api/athletes",
                new Athlete { Name = name });

            return RedirectToPage();
        }
    }
}