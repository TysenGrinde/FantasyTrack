using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; // Add this namespace

namespace FantasySports.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // League relationship
        public int LeagueId { get; set; }
        public League League { get; set; } = null!;

        // Owner relationship
        public string? OwnerId { get; set; }
        public ApplicationUser? Owner { get; set; }

        // Athletes on this team (max 8)
        public ICollection<Athlete> Athletes { get; set; } = new List<Athlete>();

        public int Wins { get; set; }
        public int Losses { get; set; }

        [NotMapped]
        public bool IsFull => Athletes.Count >= 8;
    }
}
