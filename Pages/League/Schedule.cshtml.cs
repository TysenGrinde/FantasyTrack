using Microsoft.AspNetCore.Mvc.RazorPages;
using FantasySports.Data;
using FantasySports.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace FantasySports.Pages.League
{
    public class ScheduleModel(AppDbContext _context) : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public int LeagueId { get; set; }

        public List<MeetSchedule> UpcomingMeets { get; set; } = [];

        public async Task OnGetAsync()
        {
            UpcomingMeets = await _context.MeetSchedules
                .Where(m => m.LeagueId == LeagueId)
                .OrderBy(m => m.Date)
                .ToListAsync();
        }
    }
}