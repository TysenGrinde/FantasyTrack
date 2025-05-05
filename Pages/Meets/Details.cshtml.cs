using FantasySports.Models;
using FantasySports.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FantasySports.Pages.Meets
{
    public class DetailsModel : PageModel
    {
        private readonly MeetService _meetService;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } = 1;

        public Meet Meet { get; set; }
        public bool MeetNotFound { get; set; }

        public DetailsModel(MeetService meetService)
        {
            _meetService = meetService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Meet = await _meetService.GetMeetWithResults(Id);

            if (Meet == null)
            {
                MeetNotFound = true;
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostFinalizeAsync()
        {
            Meet = await _meetService.GetMeetWithResults(Id);

            if (Meet == null)
            {
                MeetNotFound = true;
                return Page();
            }

            await _meetService.FinalizeMeet(Id);
            return RedirectToPage("./Details", new { id = Id });
        }
    }
}
