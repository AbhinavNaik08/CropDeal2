using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CropDeal.Models;

namespace CropDeal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Domain tables
        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<PaymentEvent> PaymentEvents { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Crop>()
                .Property(c => c.ExpectedPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Dealer)
                .WithMany(d => d.Subscriptions)
                .HasForeignKey(s => s.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Dealer)
                .WithMany(d => d.Transactions)
                .HasForeignKey(t => t.DealerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}