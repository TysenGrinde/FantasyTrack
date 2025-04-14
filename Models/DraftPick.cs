namespace FantasySports.Models
{
    public enum DraftStatus
    {
        NotStarted,
        InProgress,
        Completed
    }

    public class DraftPick
    {
        public int Id { get; set; }
        public int LeagueId { get; set; }
        public int TeamId { get; set; }
        public int AthleteId { get; set; }
        public int PickNumber { get; set; }
    }
}
