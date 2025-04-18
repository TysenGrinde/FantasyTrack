using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; // Add this namespace

namespace FantasySports.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // League relationship
        public int LeagueId { get; set; }
        public League League { get; set; } = null!;

        // Owner relationship
        public string OwnerId { get; set; } = null!;
        public ApplicationUser Owner { get; set; } = null!;

        // Athletes on this team (max 8)
        public ICollection<Athlete> Athletes { get; set; } = new List<Athlete>();

        [NotMapped]
        public bool IsFull => Athletes.Count >= 8;
    }
}
