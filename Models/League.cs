using System.ComponentModel.DataAnnotations.Schema;

namespace FantasySports.Models
{
    public class League
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Creator relationship
        public string CreatorId { get; set; } = null!;
        public ApplicationUser Creator { get; set; } = null!;

        // Members (users who joined)
        public ICollection<LeagueMember> Members { get; set; } = new List<LeagueMember>();

        // Teams in this league (max 8)
        public ICollection<Team> Teams { get; set; } = new List<Team>();

        [NotMapped]
        public bool IsFull => Teams.Count >= 8;
    }
}
