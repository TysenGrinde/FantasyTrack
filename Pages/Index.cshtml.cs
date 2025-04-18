using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FantasySports.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace FantasySports.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<Athlete> Athletes { get; set; } = new();

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Athlete>>("https://localhost:5001/api/athletes");
                Athletes = response ?? new List<Athlete>();
            }
            catch (HttpRequestException)
            {
                Athletes = new List<Athlete>();
            }
        }

        public async Task<IActionResult> OnPostAsync(string name, string sport)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(sport))
            {
                return Page();
            }

            await _httpClient.PostAsJsonAsync("https://localhost:5001/api/athletes",
                new Athlete { Name = name });

            return RedirectToPage();
        }
    }
}