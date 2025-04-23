using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FantasySports.Data;
using FantasySports.Services;
using FantasySports.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environments.Development // Force development environment
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

// Development configuration
if (builder.Environment.IsDevelopment())
{
    // Enable runtime compilation for Hot Reload
    builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
}

// Add SQLite database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors());

// Add Identity with Entity Framework stores - THIS REPLACES THE SEPARATE AUTHENTICATION CONFIG
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Add authorization
builder.Services.AddAuthorization();

// Add TeamService and LeagueService registration
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<LeagueService>();

// Add CSV import service
builder.Services.AddScoped<CsvImportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Initialize and seed the database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            // Use Migrations instead of EnsureCreated for better control
            context.Database.Migrate();

            // Seed initial data if needed
            // await SeedData.Initialize(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();

app.MapGet("/testdb", async (AppDbContext db) =>
{
    var stats = new
    {
        Users = await db.Users.CountAsync(),
        Teams = await db.Teams.CountAsync(),
        Athletes = await db.Athletes.CountAsync()
    };
    return Results.Ok(stats);
});

app.Run();