namespace FantasySports.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } // "100m Dash", "Long Jump", etc.
        public string Unit { get; set; } // "seconds", "meters", etc.
    }
}
