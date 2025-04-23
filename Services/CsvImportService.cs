// Services/CsvImportService.cs
using CsvHelper;
using CsvHelper.Configuration;
using FantasySports.Data;
using FantasySports.Models;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class CsvImportService
{
    private readonly AppDbContext _context;

    public CsvImportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task ImportAthletesFromCsv(Stream fileStream)
    {
        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        csv.Context.RegisterClassMap<AthleteCsvMap>();
        var records = csv.GetRecords<Athlete>().ToList();

        await _context.Athletes.AddRangeAsync(records);
        await _context.SaveChangesAsync();
    }
}

public sealed class AthleteCsvMap : ClassMap<Athlete>
{
    public AthleteCsvMap()
    {
        Map(m => m.Id).Name("Id");
        Map(m => m.ImageURL).Name("ImageURL");
        Map(m => m.Name).Name("Name");
        Map(m => m.Position).Name("Position");
    }
}