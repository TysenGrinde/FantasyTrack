using FantasySports.Data;
using FantasySports.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FantasySports.Services
{
    public class Result
    {
        public bool Succeeded { get; }
        public string? Error { get; }
        public object? Data { get; }

        private Result(bool succeeded, string? error, object? data)
        {
            Succeeded = succeeded;
            Error = error;
            Data = data;
        }

        public static Result Success(object? data = null) => new(true, null, data);
        public static Result Failure(string error) => new(false, error, null);
    }
    public class LeagueService
    {
        private readonly AppDbContext _context;

        public LeagueService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> CreateLeague(string creatorId, string leagueName)
        {
            if (await _context.Leagues.AnyAsync(l => l.CreatorId == creatorId))
            {
                return Result.Failure("You can only create one league");
            }

            var league = new League
            {
                Name = leagueName,
                CreatorId = creatorId
            };

            _context.Leagues.Add(league);
            await _context.SaveChangesAsync();

            return Result.Success(league.Id);
        }

        public async Task<Result> JoinLeague(string userId, int leagueId)
        {
            var league = await _context.Leagues
                .Include(l => l.Members)
                .Include(l => l.Teams)
                .FirstOrDefaultAsync(l => l.Id == leagueId);

            if (league == null) return Result.Failure("League not found");
            if (league.Teams.Count >= 8) return Result.Failure("League is full");

            var userLeaguesCount = await _context.LeagueMembers
                .CountAsync(lm => lm.UserId == userId);

            if (userLeaguesCount >= 4)
            {
                return Result.Failure("You can only join 4 leagues");
            }

            if (league.Members.Any(m => m.UserId == userId))
            {
                return Result.Failure("You already joined this league");
            }

            var member = new LeagueMember
            {
                UserId = userId,
                LeagueId = leagueId
            };

            _context.LeagueMembers.Add(member);

            var team = new Team
            {
                Name = $"{userId}'s Team",
                LeagueId = leagueId,
                OwnerId = userId
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
    public class TeamService
    {
        private readonly AppDbContext _context;

        public TeamService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> AddAthlete(int teamId, int athleteId)
        {
            var team = await _context.Teams
                .Include(t => t.Athletes)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null) return Result.Failure("Team not found");
            if (team.Athletes.Count >= 8) return Result.Failure("Team already has 8 athletes");

            var athlete = await _context.Athletes.FindAsync(athleteId);
            if (athlete == null) return Result.Failure("Athlete not found");

            athlete.TeamId = teamId;
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }

}