using FantasySports.Models;
using FantasySports.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace FantasySports.Pages.Meets
{
    public class CreateModel : PageModel
    {
        private readonly MeetService _meetService;

        [BindProperty]
        public string MeetName { get; set; }

        [BindProperty]
        public DateTime MeetDate { get; set; } = DateTime.Today;

        [BindProperty]
        public List<int> SelectedEventIds { get; set; } = new();

        public List<Event> AvailableEvents { get; set; }

        public CreateModel(MeetService meetService)
        {
            _meetService = meetService;
        }

        public async Task OnGetAsync()
        {
            AvailableEvents = await _meetService.GetAvailableEvents();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AvailableEvents = await _meetService.GetAvailableEvents();
                return Page();
            }

            var meet = await _meetService.CreateMeet(MeetName, MeetDate, SelectedEventIds);
            return RedirectToPage("./Details", new { id = meet.Id });
        }
    }
}
