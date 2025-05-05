// Services/LeagueService.cs
using FantasySports.Data;
using FantasySports.Models;
using FantasySports.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class LeagueService
{
    private readonly AppDbContext _context;

    public LeagueService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> CreateLeague(string creatorId, string leagueName)
    {
        try
        {
            // Check if user already has a league
            var existingLeague = await _context.Leagues
                .FirstOrDefaultAsync(l => l.CreatorId == creatorId);

            if (existingLeague != null)
            {
                // Option 1: Update existing league
                existingLeague.Name = leagueName;
                existingLeague.CreatedDate = DateTime.UtcNow;

                // Option 2: Return error if you want one league per user
                // return Result.Failure("You can only create one league");
            }
            else
            {
                // Create new league
                var league = new League
                {
                    Name = leagueName,
                    CreatorId = creatorId,
                    CreatedDate = DateTime.UtcNow,
                    TradeLimit = 3,
                    TradeReviewPeriodHours = 24,
                    IsDraftComplete = false,
                    IsDraftStarted = false,
                    DraftOrder = string.Empty
                };

                // Add 8 teams to the league
                for (int i = 1; i <= 8; i++)
                {
                    league.Teams.Add(new Team
                    {
                        Name = $"Team {i}",
                        // Set other team properties as needed
                        CreatedDate = DateTime.UtcNow,
                        OwnerId = null, // Or assign owners if available
                        LogoUrl = $"/images/teams/team-{i}.png" // Example
                    });
                }

                _context.Leagues.Add(league);
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (DbUpdateException ex)
        {
            // Log the full error
            Console.WriteLine($"Database error: {ex}");

            if (ex.InnerException is SqliteException sqlEx)
            {
                switch (sqlEx.SqliteErrorCode)
                {
                    case 19: // Constraint violation
                        return Result.Failure("You already have a league created");
                    case 5: // Database locked
                        return Result.Failure("Database is busy. Please try again.");
                    default:
                        return Result.Failure("Database error occurred");
                }
            }

            return Result.Failure("An error occurred while saving the league");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex}");
            return Result.Failure("An unexpected error occurred");
        }
    }
}