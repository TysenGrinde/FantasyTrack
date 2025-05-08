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
                Events = eventIds.Select(e => new MeetEvent { EventId = e }).ToList(),
                LeagueId = 2 // Assuming a default league ID for now
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
                    .ThenInclude(e => e.Event)
                .Include(m => m.Events)
                    .ThenInclude(e => e.Performances)
                        .ThenInclude(p => p.Athlete)
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
                TeamIdAtTime = athlete?.TeamId ?? -1,
                PointsEarned = 0 // Will be calculated later
            };

            _context.AthletePerformances.Add(performance);
            await _context.SaveChangesAsync();
        }

        public async Task<Meet> GetMeetWithResults(int meetId)
        {
            return await _context.Meets
                .AsNoTracking() // Prevent caching issues
                .Include(m => m.Events)
                    .ThenInclude(e => e.Event)
                .Include(m => m.Events)
                    .ThenInclude(e => e.Performances)
                        .ThenInclude(p => p.Athlete)
                .Include(m => m.TeamScores)
                    .ThenInclude(ts => ts.Team)
                .Include(m => m.Matchups)
                    .ThenInclude(m => m.HomeTeam)
                .Include(m => m.Matchups)
                    .ThenInclude(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == meetId);
        }

        public async Task FinalizeMeet(int meetId)
        {
            // Use a transaction for the entire operation
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Load ALL required data in a single query
                var meet = await _context.Meets
                    .Include(m => m.Events)
                        .ThenInclude(e => e.Event)
                    .Include(m => m.Events)
                        .ThenInclude(e => e.Performances)
                    .Include(m => m.TeamScores)
                        .ThenInclude(ts => ts.Team)
                    .Include(m => m.Matchups)
                        .ThenInclude(m => m.HomeTeam)
                    .Include(m => m.Matchups)
                        .ThenInclude(m => m.AwayTeam)
                    .FirstOrDefaultAsync(m => m.Id == meetId);

                if (meet == null)
                    throw new ArgumentException($"Meet {meetId} not found");

                if (meet.IsCompleted)
                {
                    _logger.LogInformation($"Meet {meetId} already finalized");
                    return;
                }

                // Calculate scores
                var scoringService = new MeetScoringService(_context,
                    _context.GetService<ILogger<MeetScoringService>>());
                await scoringService.CalculateMeetScores(meetId, transaction);

                // In the FinalizeMeet method, replace the team record update logic with:
                var teamService = new TeamService(_context);
                foreach (var matchup in meet.Matchups)
                {
                    var homeScore = meet.TeamScores
                        .FirstOrDefault(ts => ts.TeamId == matchup.HomeTeamId)?.TotalPoints ?? 0;
                    var awayScore = meet.TeamScores
                        .FirstOrDefault(ts => ts.TeamId == matchup.AwayTeamId)?.TotalPoints ?? 0;

                    matchup.HomeTeamScore = homeScore;
                    matchup.AwayTeamScore = awayScore;

                    // Use the TeamService to update records
                    await teamService.ProcessMatchupResult(
                        matchup.HomeTeamId,
                        matchup.AwayTeamId,
                        homeScore,
                        awayScore);
                }

                // Finalize meet
                meet.IsCompleted = true;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Successfully finalized meet {meetId}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error finalizing meet {meetId}");
                throw;
            }
        }

        public async Task ScheduleMeetMatchups(int meetId)
        {
            var meet = await _context.Meets
                .Include(m => m.League)
                    .ThenInclude(l => l.Teams)
                .FirstOrDefaultAsync(m => m.Id == meetId);

            if (meet == null || meet.League?.Teams == null)
                throw new Exception("Meet or teams not found");

            var teams = meet.League.Teams.ToList();
            var matchups = new List<TeamMatchup>();

            // Simple round-robin pairing
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    matchups.Add(new TeamMatchup
                    {
                        MeetId = meetId,
                        HomeTeamId = teams[i].Id,
                        AwayTeamId = teams[j].Id
                    });
                }
            }

            _context.TeamMatchups.AddRange(matchups);
            await _context.SaveChangesAsync();
        }
    }
}
