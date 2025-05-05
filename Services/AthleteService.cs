namespace FantasySports.Services;
using FantasySports.Data;
using FantasySports.Models;
using Microsoft.EntityFrameworkCore;


public class AthleteService
{
    private readonly AppDbContext _context;

    public AthleteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Athlete>> GetAvailableAthletes()
    {
        return await _context.Athletes
            .Include(a => a.Team)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Athlete> GetAthleteDetails(int athleteId)
    {
        return await _context.Athletes
            .Include(a => a.Team)
            .FirstOrDefaultAsync(a => a.Id == athleteId);
    }
}
