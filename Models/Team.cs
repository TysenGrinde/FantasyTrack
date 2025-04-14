namespace FantasySports.Models
{
    public class Team
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<Athlete> Athletes { get; set; } = new();
        public string UserId { get; set; } // Links to IdentityUser
        public int LeagueId { get; set; } // Tracks which league the team belongs to 
    }
}
