using System.ComponentModel.DataAnnotations.Schema;

namespace FantasySports.Models
{
    public class Athlete
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sport { get; set; } = string.Empty;

        // Team relationship
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
    }
}