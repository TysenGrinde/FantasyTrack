namespace FantasySports.Models
{
    public class MeetEvent
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public int MeetId { get; set; }
        public Meet Meet { get; set; }
        public ICollection<AthletePerformance> Performances { get; set; } = new List<AthletePerformance>();
    }
}
