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
        public int Id { get; set; }

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
                .Include(m => m.Events)  // Load Events
                    .ThenInclude(e => e.Event)  // Load Event details
                .Include(m => m.Events)  // Load Events again for Performances
                    .ThenInclude(e => e.Performances)  // Load Performances
                        .ThenInclude(p => p.Athlete)  // Load Athlete for each Performance
                            .ThenInclude(a => a.Team)  // Load Team for Athlete
                .Include(m => m.Matchups)
                    .ThenInclude(m => m.HomeTeam)
                .Include(m => m.Matchups)
                    .ThenInclude(m => m.AwayTeam)
                .Include(m => m.TeamScores)
                    .ThenInclude(ts => ts.Team)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Meet == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostFinalizeAsync(int id)
        {
            try
            {
                // Get fresh meet data
                Meet = await _meetService.GetMeetWithResults(id);

                if (Meet == null)
                {
                    MeetNotFound = true;
                    return Page();
                }

                await _meetService.FinalizeMeet(id);
                TempData["SuccessMessage"] = "Meet successfully finalized!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to finalize meet: {ex.Message}";
            }

            return RedirectToPage("./Details", new { id });
        }

        public async Task<IActionResult> OnPostScheduleMatchups(int meetId)
        {
            var service = new MeetSchedulingService(_context);
            await service.ScheduleMeetMatchups(meetId);
            return RedirectToPage("./Details", new { id = meetId });
        }
    }
}
