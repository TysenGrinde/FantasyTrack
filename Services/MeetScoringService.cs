using FantasySports.Data;
using FantasySports.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FantasySports.Services
{
    public class MeetScoringService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MeetScoringService> _logger;

        public MeetScoringService(AppDbContext context, ILogger<MeetScoringService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CalculateMeetScores(int meetId, IDbContextTransaction transaction = null)
        {
            try
            {
                // Load meet with all required relationships
                var meet = await _context.Meets
                    .Include(m => m.Events)
                        .ThenInclude(e => e.Event) // Crucial - was missing before
                    .Include(m => m.Events)
                        .ThenInclude(e => e.Performances)
                    .Include(m => m.TeamScores)
                    .FirstOrDefaultAsync(m => m.Id == meetId);

                if (meet == null)
                    throw new ArgumentException($"Meet {meetId} not found");

                // Clear existing scores
                var existingScores = _context.TeamScores.Where(ts => ts.MeetId == meetId);
                _context.TeamScores.RemoveRange(existingScores);

                // Calculate scores for each event
                foreach (var meetEvent in meet.Events)
                {
                    if (meetEvent.Event == null)
                    {
                        _logger.LogWarning($"Event not loaded for MeetEvent {meetEvent.Id}");
                        continue;
                    }

                    await CalculateEventScores(meetEvent);
                }

                // Calculate team totals
                var teamScores = meet.Events
                    .SelectMany(e => e.Performances)
                    .GroupBy(p => p.TeamIdAtTime)
                    .Select(g => new TeamScore
                    {
                        TeamId = g.Key,
                        MeetId = meetId,
                        TotalPoints = g.Sum(p => p.PointsEarned),
                        DateCreated = DateTime.UtcNow
                    })
                    .OrderByDescending(ts => ts.TotalPoints)
                    .ToList();

                // Assign placements (handling ties)
                int currentPlace = 1;
                for (int i = 0; i < teamScores.Count; i++)
                {
                    if (i > 0 && teamScores[i].TotalPoints == teamScores[i - 1].TotalPoints)
                    {
                        teamScores[i].Placement = teamScores[i - 1].Placement;
                    }
                    else
                    {
                        teamScores[i].Placement = currentPlace;
                    }
                    currentPlace++;
                }

                await _context.TeamScores.AddRangeAsync(teamScores);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calculating scores for meet {meetId}");
                throw;
            }
        }

        private async Task CalculateEventScores(MeetEvent meetEvent)
        {
            // Add null check for meetEvent.Event
            if (meetEvent?.Event == null)
            {
                _logger.LogWarning("MeetEvent or Event is null");
                return;
            }

            var isTimedEvent = meetEvent.Event.Unit == "seconds";
            var orderedPerformances = isTimedEvent
                ? meetEvent.Performances.OrderBy(p => p.Result)
                : meetEvent.Performances.OrderByDescending(p => p.Result);

            int[] pointDistribution = { 10, 8, 6, 5, 4, 3, 2, 1 };
            int position = 0;

            foreach (var performance in orderedPerformances)
            {
                performance.PointsEarned = position < pointDistribution.Length
                    ? pointDistribution[position]
                    : 0;
                position++;
            }

            await _context.SaveChangesAsync();
        }
    }
}