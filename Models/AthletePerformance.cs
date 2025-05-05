namespace FantasySports.Models
{
    public class AthletePerformance
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public Athlete Athlete { get; set; }
        public int MeetEventId { get; set; }
        public MeetEvent MeetEvent { get; set; }
        public decimal Result { get; set; } // Time in seconds, distance in meters, etc.
        public int PointsEarned { get; set; }
        public int TeamIdAtTime { get; set; } // Snapshot of team affiliation
    }
}
