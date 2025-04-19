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
    public DbSet<Trade> Trades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // League Creator (one-to-one)
        modelBuilder.Entity<League>()
            .HasOne(l => l.Creator)
            .WithOne(u => u.CreatedLeague)
            .HasForeignKey<League>(l => l.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

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

        // Team constraints
        modelBuilder.Entity<Team>()
            .HasOne(t => t.League)
            .WithMany(l => l.Teams)
            .HasForeignKey(t => t.LeagueId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.Owner)
            .WithMany(u => u.Teams)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Athlete constraints
        modelBuilder.Entity<Athlete>()
            .HasOne(a => a.Team)
            .WithMany(t => t.Athletes)
            .HasForeignKey(a => a.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
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
    }
}