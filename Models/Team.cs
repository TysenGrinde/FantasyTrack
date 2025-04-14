namespace FantasySports.Models
{
    public class Team
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<Athlete> Athletes { get; set; } = new();
        public int LeagueId { get; set; } // Tracks which league the team belongs to 
    }
}
