namespace FantasySports.Models
{
    public class MeetSchedule
    {
        public int Id { get; set; }
        public int LeagueId { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;

        // Navigation
        public League League { get; set; } = null!;
    }
}