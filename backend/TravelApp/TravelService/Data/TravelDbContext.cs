using Microsoft.EntityFrameworkCore;
using TravelService.Models;

namespace TravelService.Data
{
    public class TravelDbContext : DbContext
    {
        public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options) { }

        public DbSet<TravelPlan> TravelPlans { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ShareToken> ShareTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TravelPlan>()
                .HasMany(t => t.Destinations)
                .WithOne(d => d.TravelPlan)
                .HasForeignKey(d => d.TravelPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TravelPlan>()
                .HasMany(t => t.Activities)
                .WithOne(a => a.TravelPlan)
                .HasForeignKey(a => a.TravelPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TravelPlan>()
                .HasMany(t => t.ShareTokens)
                .WithOne(s => s.TravelPlan)
                .HasForeignKey(s => s.TravelPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TravelPlan>()
                .Property(t => t.PlannedBudget)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Activity>()
                .Property(a => a.EstimatedCost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Activity>()
                .Property(a => a.Status)
                .HasDefaultValue("planned");

            modelBuilder.Entity<ShareToken>()
                .HasIndex(s => s.Token)
                .IsUnique();
        }
    }
}