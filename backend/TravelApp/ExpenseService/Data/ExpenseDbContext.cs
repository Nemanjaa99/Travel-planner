using Microsoft.EntityFrameworkCore;
using ExpenseService.Models;

namespace ExpenseService.Data
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options) { }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ChecklistItem> ChecklistItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Expense>()
                .HasIndex(e => new { e.TravelPlanId, e.UserId });

            modelBuilder.Entity<ChecklistItem>()
                .HasIndex(c => new { c.TravelPlanId, c.UserId });
        }
    }
}
