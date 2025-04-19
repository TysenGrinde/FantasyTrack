using System.ComponentModel.DataAnnotations.Schema;

namespace FantasySports.Models
{
    public class League
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CreatorId { get; set; } = null!;
        public ApplicationUser Creator { get; set; } = null!;
        public ICollection<LeagueMember> Members { get; set; } = new List<LeagueMember>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();

        // Draft/Trade Properties
        public DateTime? DraftDate { get; set; }
        public bool IsDraftStarted { get; set; } = false;
        public bool IsDraftComplete { get; set; } = false;
        public int TradeDeadlineDays { get; set; } = 14; // Default: 2 weeks after draft
        public string DraftOrder { get; set; } = ""; // Store as JSON or comma-separated team IDs

        [NotMapped]
        public bool IsFull => Teams.Count >= 8;
    }
}
