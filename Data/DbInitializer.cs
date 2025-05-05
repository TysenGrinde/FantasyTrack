using FantasySports.Data;
using FantasySports.Models;
using Microsoft.EntityFrameworkCore;

namespace FantasySports.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Check if the Events table is already populated
            if (context.Events.Any())
            {
                return;   // DB has been seeded
            }

            var events = new Event[]
            {
                new Event { Name = "Javelin", Unit = "meters" },
                new Event { Name = "Triple Jump", Unit = "meters" },
                new Event { Name = "Hammer", Unit = "meters" },
                new Event { Name = "Pole Vault", Unit = "meters" },
                new Event { Name = "110m Hurdles", Unit = "seconds" },
                new Event { Name = "100m Dash", Unit = "seconds" },
                new Event { Name = "400m Dash", Unit = "seconds" },
                new Event { Name = "10000m Run", Unit = "seconds" },
                new Event { Name = "Long Jump", Unit = "meters" },
                new Event { Name = "High Jump", Unit = "meters" },
                new Event { Name = "Discus", Unit = "meters" },
                new Event { Name = "Shot Put", Unit = "meters" },
                new Event { Name = "3000m Steeplechase", Unit = "seconds" },
                new Event { Name = "4x100m Relay", Unit = "seconds" },
                new Event { Name = "1500m Run", Unit = "seconds" },
                new Event { Name = "800m", Unit = "seconds" },
                new Event { Name = "400m Hurdles", Unit = "seconds" },
                new Event { Name = "200m Dash", Unit = "seconds" },
                new Event { Name = "5000m", Unit = "seconds" },
                new Event { Name = "4x400m Relay", Unit = "seconds" }
            };

            context.Events.AddRange(events);
            context.SaveChanges();
        }
    }
}