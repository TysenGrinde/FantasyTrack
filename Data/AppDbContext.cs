using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FantasySports.Models;
using Microsoft.AspNetCore.Identity;

namespace FantasySports.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Athlete> Athletes { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<League> Leagues { get; set; }
    public DbSet<LeagueMember> LeagueMembers { get; set; }
    public DbSet<MeetSchedule> MeetSchedules { get; set; }
    public DbSet<Trade> Trades { get; set; } = null!;
    public DbSet<Meet> Meets { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<MeetEvent> MeetEvents { get; set; }
    public DbSet<AthletePerformance> AthletePerformances { get; set; }
    public DbSet<TeamScore> TeamScores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // League Creator (one-to-one)
        modelBuilder.Entity<League>()
            .HasIndex(l => l.CreatorId);

        // League to Teams (one-to-many)
        modelBuilder.Entity<League>()
            .HasMany(l => l.Teams)
            .WithOne(t => t.League)
            .HasForeignKey(t => t.LeagueId);

        // League Members (many-to-many with join table)
        modelBuilder.Entity<LeagueMember>()
            .HasKey(lm => new { lm.UserId, lm.LeagueId });

        modelBuilder.Entity<LeagueMember>()
            .HasOne(lm => lm.User)
            .WithMany(u => u.JoinedLeagues)
            .HasForeignKey(lm => lm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LeagueMember>()
            .HasOne(lm => lm.League)
            .WithMany(l => l.Members)
            .HasForeignKey(lm => lm.LeagueId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LeagueMember>()
            .HasOne(lm => lm.Team)
            .WithMany()
            .HasForeignKey(lm => lm.TeamId);

        // Team constraints
        modelBuilder.Entity<Team>()
            .HasOne(t => t.League)
            .WithMany(l => l.Teams)
            .HasForeignKey(t => t.LeagueId)
            .OnDelete(DeleteBehavior.Cascade);

        // Athlete constraints
        modelBuilder.Entity<Athlete>()
            .HasOne(a => a.Team)
            .WithMany(t => t.Athletes)
            .HasForeignKey(a => a.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Athlete>()
            .Property(a => a.SlotNumber)
            .HasDefaultValue(0);

        // Add this for Trade relationships
        modelBuilder.Entity<Trade>()
            .HasOne(t => t.ProposingTeam)
            .WithMany()
            .HasForeignKey(t => t.ProposingTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Trade>()
            .HasOne(t => t.ReceivingTeam)
            .WithMany()
            .HasForeignKey(t => t.ReceivingTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Meet relationships
        modelBuilder.Entity<Meet>()
            .HasMany(m => m.Events)
            .WithOne(me => me.Meet)
            .HasForeignKey(me => me.MeetId);

        modelBuilder.Entity<Meet>()
            .HasMany(m => m.TeamScores)
            .WithOne(ts => ts.Meet)
            .HasForeignKey(ts => ts.MeetId);

        // MeetEvent relationships
        modelBuilder.Entity<MeetEvent>()
            .HasOne(me => me.Event)
            .WithMany()
            .HasForeignKey(me => me.EventId);

        modelBuilder.Entity<MeetEvent>()
            .HasMany(me => me.Performances)
            .WithOne(p => p.MeetEvent)
            .HasForeignKey(p => p.MeetEventId);

        // TeamScore relationships
        modelBuilder.Entity<TeamScore>()
            .HasOne(ts => ts.Team)
            .WithMany()
            .HasForeignKey(ts => ts.TeamId);
    }
}