using FantasySports.Data;
using FantasySports.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FantasySports.Services
{
    public class MeetService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MeetService> _logger;

        public MeetService(AppDbContext context, ILogger<MeetService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Meet> CreateMeet(string name, DateTime date, List<int> eventIds)
        {
            var meet = new Meet
            {
                Name = name,
                Date = date,
                IsCompleted = false,
                Events = eventIds.Select(e => new MeetEvent { EventId = e }).ToList()
            };

            _context.Meets.Add(meet);
            await _context.SaveChangesAsync();
            return meet;
        }

        public async Task<List<Event>> GetAvailableEvents()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<List<Meet>> GetAllMeets()
        {
            return await _context.Meets
                .Include(m => m.Events)
                    .ThenInclude(me => me.Event)
                .Include(m => m.TeamScores)
                    .ThenInclude(ts => ts.Team)
                .ToListAsync();
        }

        public async Task<Meet> GetMeetWithEvents(int meetId)
        {
            return await _context.Meets
                .Include(m => m.Events)
                    .ThenInclude(me => me.Event)
                .FirstOrDefaultAsync(m => m.Id == meetId);
        }

        public async Task RecordPerformance(int meetEventId, int athleteId, decimal result)
        {
            var athlete = await _context.Athletes.FindAsync(athleteId);

            var performance = new AthletePerformance
            {
                MeetEventId = meetEventId,
                AthleteId = athleteId,
                Result = result,
                TeamIdAtTime = athlete?.TeamId ?? 0,
                PointsEarned = 0 // Will be calculated later
            };

            _context.AthletePerformances.Add(performance);
            await _context.SaveChangesAsync();
        }
        
        public async Task<Meet> GetMeetWithResults(int meetId)
        {
            return await _context.Meets
                .Include(m => m.Events)
                    .ThenInclude(me => me.Event)
                .Include(m => m.Events)
                    .ThenInclude(me => me.Performances)
                        .ThenInclude(p => p.Athlete)
                            .ThenInclude(a => a.Team)
                .Include(m => m.TeamScores)
                    .ThenInclude(ts => ts.Team)
                .FirstOrDefaultAsync(m => m.Id == meetId);
        }

        public async Task FinalizeMeet(int meetId)
        {
            try
            {
                var meet = await _context.Meets
                    .Include(m => m.Events)
                        .ThenInclude(me => me.Performances)
                    .FirstOrDefaultAsync(m => m.Id == meetId);

                if (meet == null)
                {
                    throw new ArgumentException($"Meet with ID {meetId} not found");
                }

                if (meet.IsCompleted)
                {
                    throw new InvalidOperationException("Meet is already finalized");
                }

                // Verify all events have performances
                foreach (var meetEvent in meet.Events)
                {
                    if (!meetEvent.Performances.Any())
                    {
                        throw new InvalidOperationException(
                            $"Event {meetEvent.Event?.Name} has no performances recorded");
                    }
                }

                var scoringServiceLogger = _context.GetService<ILogger<MeetScoringService>>();
                var scoringService = new MeetScoringService(_context, scoringServiceLogger);
                await scoringService.CalculateMeetScores(meetId);

                meet.IsCompleted = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw; // Re-throw to show error in UI
            }
        }
    }
}
