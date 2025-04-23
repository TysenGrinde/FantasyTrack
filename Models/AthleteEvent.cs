namespace FantasySports.Models
{
    public class AthleteEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Stats { get; set; } = string.Empty;

        // Link back to the athlete
        public int AthleteId { get; set; }
        public Athlete Athlete { get; set; } = null!;
    }
}