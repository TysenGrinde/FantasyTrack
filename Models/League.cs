using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FantasySports.Models
{
    public class League
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? CreatorId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DraftDate { get; set; }
        public int TradeLimit { get; set; } = 3;
        public int TradeReviewPeriodHours { get; set; } = 24;

        public string DraftOrder { get; set; } = ""; // JSON or comma-separated team IDs
        public string? InviteCode { get; set; } // JSON or comma-separated athlete IDs
        public bool IsDraftStarted { get; set; } = false;
        public bool IsDraftComplete { get; set; } = false;

        // Navigation properties
        [ForeignKey("CreatorId")]
        public ApplicationUser? Creator { get; set; } = null!;
        public ICollection<LeagueMember> Members { get; set; } = new List<LeagueMember>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Meet> Meets { get; set; } = new List<Meet>();
        public ICollection<Trade> Trades { get; set; } = new List<Trade>();
    }
}