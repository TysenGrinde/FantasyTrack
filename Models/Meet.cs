namespace FantasySports.Models
{
    public class Meet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public ICollection<MeetEvent> Events { get; set; } = new List<MeetEvent>();
        public ICollection<TeamScore> TeamScores { get; set; } = new List<TeamScore>();
    }
}
