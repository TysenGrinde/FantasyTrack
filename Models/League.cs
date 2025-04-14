namespace FantasySports.Models
{
    public class League
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<Team> Teams { get; set; } = new();
        public DraftStatus DraftStatus { get; set; } = DraftStatus.NotStarted;
        public List<Athlete> AvailableAthletes { get; set; } = new();
    }
}
