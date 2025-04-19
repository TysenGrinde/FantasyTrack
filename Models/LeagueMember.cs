namespace FantasySports.Models
{
    public class LeagueMember
    {
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public int LeagueId { get; set; }
        public League League { get; set; } = null!;

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        public bool IsAdmin { get; set; } = false;
    }
}
