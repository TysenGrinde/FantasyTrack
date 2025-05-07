using FantasySports.Models;
using FantasySports.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FantasySports.Data;

namespace FantasySports.Pages.Meets
{
    public class DetailsModel : PageModel
    {
        private readonly MeetService _meetService;
        private readonly AppDbContext _context;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } = 1;

        public Meet Meet { get; set; }
        public bool MeetNotFound { get; set; }

        public DetailsModel(MeetService meetService, AppDbContext context)
        {
            _meetService = meetService;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Meet = await _context.Meets
                .Include(m => m.Matchups)
                    .ThenInclude(m => m.HomeTeam)
                .Include(m => m.Matchups)
                    .ThenInclude(m => m.AwayTeam)
                .Include(m => m.TeamScores)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Meet == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostFinalizeAsync()
        {
            Meet = await _meetService.GetMeetWithResults(Id);

            if (Meet == null)
            {
                MeetNotFound = true;
                return Page();
            }

            await _meetService.FinalizeMeet(Id);
            return RedirectToPage("./Details", new { id = Id });
        }
        public async Task<IActionResult> OnPostScheduleMatchups(int meetId)
        {
            var service = new MeetSchedulingService(_context);
            await service.ScheduleMeetMatchups(meetId);
            return RedirectToPage("./Details", new { id = meetId });
        }
    }
}
