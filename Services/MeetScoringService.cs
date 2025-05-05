using FantasySports.Data;
using FantasySports.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task CalculateMeetScores(int meetId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var meet = await _context.Meets
                    .Include(m => m.Events)
                        .ThenInclude(me => me.Event)
                    .Include(m => m.Events)
                        .ThenInclude(me => me.Performances)
                    .FirstAsync(m => m.Id == meetId);

                // Get all teams that participated in this meet
                var participatingTeamIds = meet.Events
                    .SelectMany(e => e.Performances)
                    .Select(p => p.TeamIdAtTime)
                    .Distinct()
                    .ToList();

                // Get valid teams from database
                var validTeams = await _context.Teams
                    .Where(t => participatingTeamIds.Contains(t.Id))
                    .ToListAsync();

                // Clear any existing scores
                var existingScores = _context.TeamScores.Where(ts => ts.MeetId == meetId);
                _context.TeamScores.RemoveRange(existingScores);

                // Calculate for each event
                foreach (var meetEvent in meet.Events)
                {
                    await CalculateEventScores(meetEvent);
                }

                // Calculate team totals only for valid teams
                var teamScores = meet.Events
                    .SelectMany(e => e.Performances)
                    .Where(p => validTeams.Any(t => t.Id == p.TeamIdAtTime)) // Filter for valid teams
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
                        // Same as previous team
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
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully finalized meet {MeetId}", meetId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to finalize meet {MeetId}", meetId);
                throw;
            }
        }

        private async Task CalculateEventScores(MeetEvent meetEvent)
        {
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