using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyValuePrediction
{
    public class ApplicationContext : DbContext
    {
        public DbSet<PredictionModelError> PredictionModelErrors => Set<PredictionModelError>();
        public DbSet<PredictionInformation> Predictions => Set<PredictionInformation>();
        public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Cryptocurrency.db");
        }
    }
}
