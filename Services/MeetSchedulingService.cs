using FantasySports.Data;
using FantasySports.Models;
using Microsoft.EntityFrameworkCore;

namespace FantasySports.Services
{
    // Services/MeetSchedulingService.cs
    public class MeetSchedulingService
    {
        private readonly AppDbContext _context;
        private readonly Random _random = new();

        public MeetSchedulingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task ScheduleMeetMatchups(int meetId)
        {
            var meet = await _context.Meets
                .Include(m => m.League)
                .ThenInclude(l => l.Teams)
                .FirstOrDefaultAsync(m => m.Id == meetId);

            if (meet?.League?.Teams == null)
                throw new Exception("Meet or league teams not found");

            var teams = meet.League.Teams.ToList();
            Shuffle(teams);

            // Clear existing matchups
            var existing = _context.TeamMatchups.Where(t => t.MeetId == meetId);
            _context.TeamMatchups.RemoveRange(existing);

            // Create new matchups (pairing teams)
            for (int i = 0; i < teams.Count; i += 2)
            {
                if (i + 1 >= teams.Count) break; // Handle odd number of teams

                _context.TeamMatchups.Add(new TeamMatchup
                {
                    MeetId = meetId,
                    HomeTeamId = teams[i].Id,
                    AwayTeamId = teams[i + 1].Id
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task FinalizeMeetResults(int meetId)
        {
            var meet = await _context.Meets
                .Include(m => m.TeamScores)
                .Include(m => m.Matchups)
                .FirstOrDefaultAsync(m => m.Id == meetId);

            if (meet == null) return;

            foreach (var matchup in meet.Matchups.Where(m => !m.IsCompleted))
            {
                var homeScore = meet.TeamScores.FirstOrDefault(ts => ts.TeamId == matchup.HomeTeamId)?.TotalPoints ?? 0;
                var awayScore = meet.TeamScores.FirstOrDefault(ts => ts.TeamId == matchup.AwayTeamId)?.TotalPoints ?? 0;

                matchup.HomeTeamScore = homeScore;
                matchup.AwayTeamScore = awayScore;
            }

            await _context.SaveChangesAsync();
        }

        private void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}
