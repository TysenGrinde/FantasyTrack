using FantasySports.Data;
using FantasySports.Models;
using FantasySports.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FantasySports.Pages.Leagues
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly LeagueService _leagueService;

        [BindProperty]
        public LeagueInputModel League { get; set; } = new LeagueInputModel();

        public CreateModel(AppDbContext context, LeagueService leagueService)
        {
            _context = context;
            _leagueService = leagueService;
        }

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var result = await _leagueService.CreateLeague(userId, League.Name);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Error!);
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }

    public class LeagueInputModel
    {
        public string Name { get; set; } = "";
    }
}