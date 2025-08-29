using Microsoft.EntityFrameworkCore;
using PublicUtilities.Models;

namespace PublicUtilities.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Metric> Metrics { get; set; }
    public DbSet<Tariff> Tariffs { get; set; }
    public DbSet<ResidentPeriod> ResidentPeriods { get; set; }
    public DbSet<CalculationResult> CalculationResults { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.PersonalAccount)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(u => u.Metrics)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.СalculationResults)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.ResidentPeriods)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
