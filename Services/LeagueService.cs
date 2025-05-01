// Services/LeagueService.cs
using FantasySports.Data;
using FantasySports.Models;
using FantasySports.Services;

public class LeagueService
{
    private readonly AppDbContext _context;

    public LeagueService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> CreateLeague(string userId, string leagueName)
    {
        if (string.IsNullOrWhiteSpace(leagueName))
        {
            return Result.Failure("League name cannot be empty");
        }

        var league = new League
        {
            Name = leagueName,
            CreatorId = userId
        };

        // Add creator as first member
        league.Members.Add(new LeagueMember
        {
            UserId = userId,
            IsAdmin = true
        });

        _context.Leagues.Add(league);
        await _context.SaveChangesAsync();

        return Result.Success(league.Id);
    }
}