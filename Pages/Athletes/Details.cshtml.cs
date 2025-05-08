// Pages/Athletes/Details.cshtml.cs
using FantasySports.Data;
using FantasySports.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasySports.Pages.Athletes
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public Athlete Athlete { get; set; }
        public List<AthletePerformance> Performances { get; set; }

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Athlete = await _context.Athletes
                .Include(a => a.Team)
                .Include(a => a.Performances)
                    .ThenInclude(p => p.MeetEvent)
                        .ThenInclude(me => me.Event)
                    .Include(a => a.Performances)
                        .ThenInclude(p => p.MeetEvent)
                            .ThenInclude(me => me.Meet)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Athlete == null)
            {
                return NotFound();
            }

            Performances = Athlete.Performances
                .OrderByDescending(p => p.MeetEvent.Meet.Date)
                .ToList();

            return Page();
        }
    }
}