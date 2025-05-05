using FantasySports.Models;
using FantasySports.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FantasySports.Pages.Meets
{
    // Pages/Meets/EnterPerformance.cshtml.cs
    public class EnterPerformanceModel : PageModel
    {
        private readonly MeetService _meetService;
        private readonly AthleteService _athleteService;

        [BindProperty(SupportsGet = true)]
        public int MeetId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int EventId { get; set; }

        [BindProperty]
        public int AthleteId { get; set; }

        [BindProperty]
        public decimal Result { get; set; }

        public Meet Meet { get; set; }
        public MeetEvent MeetEvent { get; set; }
        public List<Athlete> AvailableAthletes { get; set; }

        public EnterPerformanceModel(MeetService meetService, AthleteService athleteService)
        {
            _meetService = meetService;
            _athleteService = athleteService;
        }

        public async Task OnGetAsync()
        {
            Meet = await _meetService.GetMeetWithEvents(MeetId);
            MeetEvent = Meet.Events.FirstOrDefault(e => e.Id == EventId);
            AvailableAthletes = await _athleteService.GetAvailableAthletes();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Meet = await _meetService.GetMeetWithEvents(MeetId);
                MeetEvent = Meet.Events.FirstOrDefault(e => e.Id == EventId);
                AvailableAthletes = await _athleteService.GetAvailableAthletes();
                return Page();
            }

            await _meetService.RecordPerformance(EventId, AthleteId, Result);
            return RedirectToPage("./EnterPerformance", new { MeetId, EventId });
        }
    }
}
