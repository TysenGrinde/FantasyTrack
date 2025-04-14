using Microsoft.AspNetCore.Mvc;
using FantasySports.Models;

namespace FantasySports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DraftController : ControllerBase
    {
        public static Random _random = new Random();

        // POST: api/draft/start
        [HttpPost("start")]
        public IActionResult StartDraft([FromBody] int leagueId)
        {
            var league = LeaguesController._leagues.FirstOrDefault(l => l.Id == leagueId);
            if (league == null) return NotFound("League not found.");

            league.DraftStatus = DraftStatus.InProgress;
            league.AvailableAthletes = new List<Athlete>(AthletesController._athletes);

            // Randomize draft order
            league.Teams = league.Teams.OrderBy(t => _random.Next()).ToList();

            return Ok($"Draft started for league {league.Name}!");
        }

        // POST: api/draft/pick
        [HttpPost("pick")]
        public IActionResult MakePick([FromBody] DraftPickRequest request)
        {
            var league = LeaguesController._leagues.FirstOrDefault(l => l.Id == request.LeagueId);
            if (league == null) return NotFound("League not found.");

            var athlete = league.AvailableAthletes.FirstOrDefault(a => a.Id == request.AthleteId);
            if (athlete == null) return NotFound("Athlete not available.");

            var team = league.Teams.FirstOrDefault(t => t.Id == request.TeamId);
            if (team == null) return NotFound("Team not found.");

            // Add athlete to team
            team.Athletes.Add(athlete);
            league.AvailableAthletes.Remove(athlete);

            return Ok($"{athlete.Name} drafted by {team.Name}!");
        }
    }

    public class DraftPickRequest
    {
        public int LeagueId { get; set; }
        public int TeamId { get; set; }
        public int AthleteId { get; set; }
    }
}