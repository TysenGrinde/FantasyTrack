using FantasySports.Data;
using FantasySports.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FantasySports.Pages.League.MyTeam
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Team MyTeam { get; set; }
        public Models.League CurrentLeague { get; set; }

        public IndexModel(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            // Find the team where the current user is the owner
            MyTeam = await _context.Teams
                .Include(t => t.League)
                .Include(t => t.Athletes)
                .FirstOrDefaultAsync(t => t.OwnerId == userId);

            if (MyTeam == null)
            {
                return NotFound("You don't have a team in any league yet.");
            }

            CurrentLeague = MyTeam.League;

            return Page();
        }

        public async Task<IActionResult> OnPostDropAthleteAsync(int athleteId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            // Find the athlete and verify it belongs to the user's team
            var athlete = await _context.Athletes
                .Include(a => a.Team)
                .FirstOrDefaultAsync(a => a.Id == athleteId && a.Team.OwnerId == userId);

            if (athlete == null)
            {
                return NotFound("Athlete not found or not on your team.");
            }

            // Remove the athlete from the team
            athlete.TeamId = null;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}