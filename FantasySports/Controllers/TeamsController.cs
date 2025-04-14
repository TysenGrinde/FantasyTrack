using Microsoft.AspNetCore.Mvc;
using FantasySports.Models;

namespace FantasySports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        public static List<Team> _teams = new();

        // GET: api/teams
        [HttpGet]
        public IActionResult GetAllTeams()
        {
            return Ok(_teams);
        }

        // POST: api/teams
        [HttpPost]
        public IActionResult CreateTeam([FromBody] Team team)
        {
            team.Id = _teams.Count + 1;
            _teams.Add(team);
            return Ok(team);
        }
    }
}