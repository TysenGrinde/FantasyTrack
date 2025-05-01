// Pages/Leagues/Create.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FantasySports.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace FantasySports.Pages.Leagues
{
    [Authorize] // Requires user to be logged in
    public class CreateModel : PageModel
    {
        private readonly LeagueService _leagueService;

        [BindProperty] // This attribute is REQUIRED
        public string LeagueName { get; set; } = string.Empty;

        public CreateModel(LeagueService leagueService)
        {
            _leagueService = leagueService;
        }

        public void OnGet()
        {
            // Initialization if needed
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge(); // Redirect to login if not authenticated
            }

            var result = await _leagueService.CreateLeague(userId, LeagueName);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                return Page();
            }

            return RedirectToPage("/Leagues/Index");
        }
    }
}