using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FantasySports.Models;
using System.Net.Http;
using System.Net.Http.Json;
using FantasySports.Data;

namespace FantasySports.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public List<Athlete> AvailableAthletes { get; set; } = new List<Athlete>();

        public async Task OnGetAsync()
        {
            // Fetch athletes where TeamId is NULL
            AvailableAthletes = await _context.Athletes
                .Where(a => a.TeamId == null)
                .ToListAsync();
        }
    }
}