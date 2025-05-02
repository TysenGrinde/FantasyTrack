// Pages/League/Members.cshtml.cs
using FantasySports.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using FantasySports.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FantasySports.Pages.League
{
    public class MembersModel(AppDbContext _context) : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public int LeagueId { get; set; }

        public List<LeagueMemberView> Members { get; set; } = [];

        public async Task OnGetAsync()
        {
            Members = await _context.LeagueMembers
                .Include(lm => lm.User)
                .Include(lm => lm.Team)
                .Where(lm => lm.LeagueId == LeagueId)
                .Select(lm => new LeagueMemberView
                {
                    TeamName = lm.Team.Name,
                    Username = lm.User.UserName,
                    Email = lm.User.Email
                })
                .ToListAsync();
        }

        public class LeagueMemberView
        {
            public string TeamName { get; set; } = null!;
            public string Username { get; set; } = null!;
            public string Email { get; set; } = null!;
        }
    }
}