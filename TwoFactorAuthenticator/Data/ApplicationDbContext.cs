using Authenticator;
using Microsoft.EntityFrameworkCore;

namespace TwoFactorAuthenticator.Data
{
    class ApplicationDbContext : DbContext
    {
        public DbSet<AccountSettings> Accounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data source=Accounts.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
