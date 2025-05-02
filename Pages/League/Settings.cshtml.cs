// Pages/League/Settings.cshtml.cs
using FantasySports.Data;
using FantasySports.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FantasySports.Pages.League
{
    [Authorize]
    public class SettingsModel(AppDbContext _context) : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public int LeagueId { get; set; }

        [BindProperty]
        public LeagueSettingsInput Input { get; set; } = new();

        public bool IsCreator { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var league = await _context.Leagues.FindAsync(LeagueId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IsCreator = league?.CreatorId == userId;
            if (!IsCreator) return Forbid();

            Input = new LeagueSettingsInput
            {
                LeagueName = league.Name,
                DraftDate = league.DraftDate ?? DateTime.Now.AddDays(7),
                TradeLimit = league.TradeLimit,
                TradeReviewPeriodHours = league.TradeReviewPeriodHours
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var league = await _context.Leagues.FindAsync(LeagueId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (league?.CreatorId != userId) return Forbid();

            league.Name = Input.LeagueName;
            league.DraftDate = Input.DraftDate;
            league.TradeLimit = Input.TradeLimit;
            league.TradeReviewPeriodHours = Input.TradeReviewPeriodHours;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index", new { leagueId = LeagueId });
        }

        public class LeagueSettingsInput
        {
            [Required(ErrorMessage = "League name is required")]
            public string LeagueName { get; set; } = string.Empty;
            public DateTime DraftDate { get; set; }
            public int TradeLimit { get; set; }
            public int TradeReviewPeriodHours { get; set; }
        }
    }
}