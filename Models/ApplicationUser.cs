using Microsoft.AspNetCore.Identity;

namespace FantasySports.Models
{
    public class ApplicationUser : IdentityUser
    {
        // A user can create ONE league
        public League? CreatedLeague { get; set; }

        // A user can join up to 4 leagues
        public ICollection<LeagueMember> JoinedLeagues { get; set; } = new List<LeagueMember>();

        // A user has one team per league they join
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
