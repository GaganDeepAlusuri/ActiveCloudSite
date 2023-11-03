using Microsoft.EntityFrameworkCore;
using CryptoPulse.Models;

namespace CryptoPulse.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Coin> Coins { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define a One-to-Many relationship between Coin and Market
            modelBuilder.Entity<Coin>()
                .HasMany(c => c.Markets)
                .WithOne(m => m.Coin)
                .HasForeignKey(m => m.CoinID);

            // Define a One-to-Many relationship between Exchange and Market
            modelBuilder.Entity<Exchange>()
                .HasMany(e => e.Markets)
                .WithOne(m => m.Exchange)
                .HasForeignKey(m => m.ExchangeID);
        }*/
    }
}
