namespace FantasySports.Models
{
    public class TeamScore
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int MeetId { get; set; }
        public Meet Meet { get; set; }
        public int TotalPoints { get; set; }
        public int Placement { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
