using System.Diagnostics;
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
                CreatorId = creatorId,
                DraftDate = DateTime.UtcNow.AddDays(7) // Default: draft in 1 week
            };

            _context.Leagues.Add(league);

            // Add creator as admin member
            _context.LeagueMembers.Add(new LeagueMember
            {
                UserId = creatorId,
                LeagueId = league.Id,
                IsAdmin = true // Add this property to LeagueMember.cs
            });

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
        public async Task<Result> DropAthlete(int teamId, int athleteId)
        {
            var athlete = await _context.Athletes.FindAsync(athleteId);
            if (athlete?.TeamId != teamId)
                return Result.Failure("Athlete not found or not on your team");

            athlete.TeamId = null;
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> PickupFreeAgent(int teamId, int athleteId)
        {
            var team = await _context.Teams.Include(t => t.Athletes).FirstOrDefaultAsync(t => t.Id == teamId);
            var athlete = await _context.Athletes.FindAsync(athleteId);

            if (team == null || athlete == null)
                return Result.Failure("Invalid team or athlete");
            if (team.Athletes.Count >= 8)
                return Result.Failure("Team is full");
            if (athlete.TeamId != null)
                return Result.Failure("Athlete is already on a team");

            athlete.TeamId = teamId;
            await _context.SaveChangesAsync();
            return Result.Success();
        }
    }
    public class DraftService
    {
        private readonly AppDbContext _context;
        public DraftService(AppDbContext context) => _context = context;

        public async Task<Result> StartDraft(int leagueId, string userId)
        {
            var league = await _context.Leagues
                .Include(l => l.Members)
                .Include(l => l.Teams)
                .FirstOrDefaultAsync(l => l.Id == leagueId);

            if (league == null) return Result.Failure("League not found");
            if (league.IsDraftStarted) return Result.Failure("Draft already started");
            if (!league.Members.Any(m => m.UserId == userId && m.IsAdmin))
                return Result.Failure("Only admins can start drafts");

            league.IsDraftStarted = true;
            league.DraftOrder = string.Join(",", GenerateSnakeOrder(league.Teams.Select(t => t.Id).ToList()));
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        private List<int> GenerateSnakeOrder(List<int> teamIds)
        {
            var order = new List<int>();
            for (int i = 0; i < teamIds.Count; i++)
                order.Add(teamIds[i]);
            for (int i = teamIds.Count - 1; i >= 0; i--)
                order.Add(teamIds[i]); // Snake back
            return order;
        }
    }
    public class TradeService
    {
        private readonly AppDbContext _context;
        public TradeService(AppDbContext context) => _context = context;

        public async Task<Result> ProposeTrade(
            int proposingTeamId,
            int receivingTeamId,
            List<int> offeredAthletes,
            List<int> requestedAthletes)
        {
            var trade = new Trade
            {
                ProposingTeamId = proposingTeamId,
                ReceivingTeamId = receivingTeamId,
                OfferedAthletes = string.Join(",", offeredAthletes),
                RequestedAthletes = string.Join(",", requestedAthletes),
                Status = TradeStatus.Pending,
                ExpiryDate = DateTime.UtcNow.AddDays(2) // 48 hours to respond
            };
            _context.Trades.Add(trade);
            await _context.SaveChangesAsync();
            return Result.Success(trade.Id);
        }
        private async Task ExecuteTrade(Trade trade)
        {
            var offeredIds = trade.OfferedAthletes.Split(',').Select(int.Parse).ToList();
            var requestedIds = trade.RequestedAthletes.Split(',').Select(int.Parse).ToList();

            // Move offered athletes to receiving team
            foreach (var athleteId in offeredIds)
            {
                var athlete = await _context.Athletes.FindAsync(athleteId);
                if (athlete != null) athlete.TeamId = trade.ReceivingTeamId;
            }

            // Move requested athletes to proposing team
            foreach (var athleteId in requestedIds)
            {
                var athlete = await _context.Athletes.FindAsync(athleteId);
                if (athlete != null) athlete.TeamId = trade.ProposingTeamId;
            }
        }

        public async Task<Result> RespondToTrade(int tradeId, int teamId, bool accept, string? counterOffer = null)
        {
            var trade = await _context.Trades.FindAsync(tradeId);
            if (trade == null) return Result.Failure("Trade not found");
            if (trade.ReceivingTeamId != teamId) return Result.Failure("Not authorized");

            if (accept)
            {
                // Swap athletes between teams
                await ExecuteTrade(trade);
                trade.Status = TradeStatus.Accepted;
            }
            else if (!string.IsNullOrEmpty(counterOffer))
            {
                trade.Status = TradeStatus.Countered;
                trade.CounterOffer = counterOffer;
            }
            else
            {
                trade.Status = TradeStatus.Declined;
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
    }
}