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

        public int LeagueId { get; set; } = 2; // Default to league ID 1
        public Models.League CurrentLeague { get; set; }

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
            CurrentLeague = await _context.Leagues
                .Include(l => l.Teams) // This is crucial to load related teams
                    .ThenInclude(t => t.Athletes) // This is crucial
                .FirstOrDefaultAsync(l => l.Id == LeagueId); // Or your league ID

            if (CurrentLeague == null)
            {
                CurrentLeague = new Models.League
                {
                    Id = LeagueId,
                    Name = "Default League",
                    // Add other required properties
                };
                _context.Leagues.Add(CurrentLeague);
                await _context.SaveChangesAsync();
            }
        }
    }
}