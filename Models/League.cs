using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FantasySports.Models
{
    public class League
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string CreatorId { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DraftDate { get; set; }
        public int TradeLimit { get; set; } = 3;
        public int TradeReviewPeriodHours { get; set; } = 24;

        public string DraftOrder { get; set; } = ""; // JSON or comma-separated team IDs
        public bool IsDraftStarted { get; set; } = false;
        public bool IsDraftComplete { get; set; } = false;

        // Navigation properties
        public ApplicationUser Creator { get; set; } = null!;
        public ICollection<LeagueMember> Members { get; set; } = new List<LeagueMember>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<MeetSchedule> MeetSchedules { get; set; } = new List<MeetSchedule>();
        public ICollection<Trade> Trades { get; set; } = new List<Trade>();
    }
}