using Microsoft.EntityFrameworkCore;
using VBanking.Domain.Entities;

namespace VBanking.Infrastructure.Data
{
    public class BankingDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountDeactivationLog> AccountDeactivationLogs { get; set; }


        public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Document)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}