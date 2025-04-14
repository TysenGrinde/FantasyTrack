using FantasySports.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FantasySports.Services
{
    public class TeamService
    {
        private readonly InMemoryUserContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TeamService(InMemoryUserContext context,
                          UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Team? GetUserTeam(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            return _context.Teams.FirstOrDefault(t => t.UserId == userId);
        }

        public IActionResult CreateTeam(string userId, string teamName)
        {
            if (_context.Teams.Any(t => t.UserId == userId))
            {
                return new BadRequestObjectResult("User already has a team");
            }

            var team = new Team { Name = teamName, UserId = userId };
            _context.Teams.Add(team);
            return new OkObjectResult(team);
        }
    }
}
