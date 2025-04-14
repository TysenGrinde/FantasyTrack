using Microsoft.AspNetCore.Mvc;
using FantasySports.Models;

namespace FantasySports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaguesController : ControllerBase
    {
        public static List<League> _leagues = new();

        // GET: api/leagues
        [HttpGet]
        public IActionResult GetAllLeagues()
        {
            return Ok(_leagues);
        }

        // POST: api/leagues
        [HttpPost]
        public IActionResult CreateLeague([FromBody] League league)
        {
            league.Id = _leagues.Count + 1;
            _leagues.Add(league);
            return Ok(league);
        }

        // POST: api/leagues/{id}/join
        [HttpPost("{id}/join")]
        public IActionResult JoinLeague(int id, [FromBody] int teamId)
        {
            var league = _leagues.FirstOrDefault(l => l.Id == id);
            if (league == null) return NotFound("League not found.");

            var team = TeamsController._teams.FirstOrDefault(t => t.Id == teamId);
            if (team == null) return NotFound("Team not found.");

            team.LeagueId = id;
            league.Teams.Add(team);
            return Ok($"Team {team.Name} joined league {league.Name}!");
        }
    }
}