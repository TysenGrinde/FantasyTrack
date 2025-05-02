// Pages/League/Index.cshtml.cs
using FantasySports.Data;
using FantasySports.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FantasySports.Pages.League
{
    public class IndexModel(AppDbContext _context) : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public int LeagueId { get; set; }

        // Explicitly qualify the League model
        public Models.League CurrentLeague { get; set; } = null!;

        public async Task OnGetAsync()
        {
            CurrentLeague = await _context.Leagues
                .Include(l => l.Teams)
                .Include(l => l.Members)
                .FirstOrDefaultAsync(l => l.Id == LeagueId);
        }
    }
}