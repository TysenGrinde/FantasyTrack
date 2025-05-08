using FantasySports.Data;
using FantasySports.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace FantasySports.Pages.League
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Models.League CurrentLeague { get; set; }
        public bool HasLeague { get; set; }
        [BindProperty]
        public string LeagueName { get; set; }
        [BindProperty]
        public string LeagueCode { get; set; }
        public List<Team> teams { get; set; } = new List<Team>();

        public IndexModel(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                HasLeague = false;
                return;
            }

            // Check if user is a creator or member of any league
            CurrentLeague = await _context.Leagues
                .Include(l => l.Teams)
                    .ThenInclude(t => t.Athletes)
                .Include(l => l.Members)
                .FirstOrDefaultAsync(l => l.CreatorId == userId || l.Members.Any(m => m.UserId == userId));

            teams = await _context.Teams
                .Include(t => t.Athletes)
                .Where(t => t.Id != -1)
                .ToListAsync();

            HasLeague = CurrentLeague != null;

            if (HasLeague)
            {
                // Load usernames for team owners
                var ownerIds = CurrentLeague.Teams
                    .Where(t => !string.IsNullOrEmpty(t.OwnerId))
                    .Select(t => t.OwnerId)
                    .Distinct()
                    .ToList();

                var users = await _userManager.Users
                    .Where(u => ownerIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.UserName);

                ViewData["UserNames"] = users;
            }
        }

        public async Task<IActionResult> OnPostCreateLeagueAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            if (string.IsNullOrWhiteSpace(LeagueName))
            {
                ModelState.AddModelError("LeagueName", "League name is required");
                return Page();
            }

            var league = new Models.League
            {
                Name = LeagueName,
                CreatorId = userId,
                // Add other default properties as needed
            };

            _context.Leagues.Add(league);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostJoinLeagueAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            if (string.IsNullOrWhiteSpace(LeagueCode))
            {
                ModelState.AddModelError("LeagueCode", "League code is required");
                return Page();
            }

            // Find league by code
            var league = await _context.Leagues
                .Include(l => l.Teams)
                .FirstOrDefaultAsync(l => l.InviteCode == LeagueCode);

            if (league == null)
            {
                ModelState.AddModelError("LeagueCode", "League not found");
                return Page();
            }

            // Check if user is already a member
            var existingMember = await _context.LeagueMembers
                .FirstOrDefaultAsync(m => m.LeagueId == league.Id && m.UserId == userId);

            if (existingMember != null)
            {
                // User is already a member
                return RedirectToPage();
            }

            // Find first available team in this league with no owner
            var availableTeam = league.Teams.FirstOrDefault(t => string.IsNullOrEmpty(t.OwnerId));

            if (availableTeam == null)
            {
                ModelState.AddModelError("", "No available teams in this league. Please ask the league admin to add more teams.");
                return Page();
            }

            // Assign team to user
            availableTeam.OwnerId = userId;

            // Create league membership
            var member = new LeagueMember
            {
                LeagueId = league.Id,
                UserId = userId,
                IsAdmin = false,
                JoinedDate = DateTime.UtcNow,
                TeamId = availableTeam.Id
            };

            try
            {
                _context.LeagueMembers.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToPage();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "An error occurred while joining the league. Please try again.");
                return Page();
            }
        }
    }
}