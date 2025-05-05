using FantasySports.Models;
using FantasySports.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace FantasySports.Pages.Meets
{
    public class IndexModel : PageModel
    {
        private readonly MeetService _meetService;

        public List<Meet> UpcomingMeets { get; set; }
        public List<Meet> CompletedMeets { get; set; }

        public IndexModel(MeetService meetService)
        {
            _meetService = meetService;
        }

        public async Task OnGetAsync()
        {
            var allMeets = await _meetService.GetAllMeets();

            UpcomingMeets = allMeets
                .Where(m => !m.IsCompleted)
                .OrderBy(m => m.Date)
                .ToList();

            CompletedMeets = allMeets
                .Where(m => m.IsCompleted)
                .OrderByDescending(m => m.Date)
                .ToList();
        }
    }
}