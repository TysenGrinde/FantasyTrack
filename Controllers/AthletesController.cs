using Microsoft.AspNetCore.Mvc;
using FantasySports.Models;

namespace FantasySports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AthletesController : ControllerBase
    {
        public static List<Athlete> _athletes = new();

        // GET: api/athletes
        [HttpGet]
        public IActionResult GetAllAthletes()
        {
            return Ok(_athletes);
        }

        // POST: api/athletes
        [HttpPost]
        public IActionResult AddAthlete([FromBody] Athlete athlete)
        {
            athlete.Id = _athletes.Count + 1;
            _athletes.Add(athlete);
            return Ok(athlete);
        }
    }
}