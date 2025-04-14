namespace FantasySports.Models
{
    public class Athlete
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Sport { get; set; }
    }
}