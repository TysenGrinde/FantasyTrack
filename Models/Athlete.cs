using System.ComponentModel.DataAnnotations.Schema;

namespace FantasySports.Models
{
    public class Athlete
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;

        // Team relationship (existing)
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public int SlotNumber { get; set; }

        // List of events (e.g., games, matches)
        public List<AthleteEvent> Events { get; set; } = new List<AthleteEvent>();
    }
}