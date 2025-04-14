using FantasySports.Models;
using FantasySports.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FantasySports.Pages
{
    [Authorize]
    public class MyTeamModel : PageModel
    {
        private readonly TeamService _teamService;

        public Team? UserTeam { get; set; }

        public MyTeamModel(TeamService teamService)
        {
            _teamService = teamService;
        }

        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                UserTeam = _teamService.GetUserTeam(userId);
            }
        }
    }
}
