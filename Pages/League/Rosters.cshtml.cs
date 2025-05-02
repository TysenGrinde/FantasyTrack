using Microsoft.AspNetCore.Mvc.RazorPages;
using FantasySports.Data;
using FantasySports.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace FantasySports.Pages.League
{
    public class RostersModel(AppDbContext _context) : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public int LeagueId { get; set; }

        public List<TeamRosterView> TeamRosters { get; set; } = [];

        public async Task OnGetAsync()
        {
            TeamRosters = await _context.Teams
                .Include(t => t.Athletes)
                .Where(t => t.LeagueId == LeagueId)
                .Select(t => new TeamRosterView
                {
                    TeamName = t.Name,
                    Athletes = t.Athletes.OrderBy(a => a.SlotNumber).ToList()
                })
                .ToListAsync();
        }

        public class TeamRosterView
        {
            public string TeamName { get; set; } = string.Empty;
            public List<Athlete> Athletes { get; set; } = [];
        }
    }
}