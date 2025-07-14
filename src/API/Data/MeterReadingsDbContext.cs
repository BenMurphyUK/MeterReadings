using MeterReadingsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MeterReadingsApi.Data
{
    public class MeterReadingsDbContext : DbContext
    {
        public MeterReadingsDbContext(DbContextOptions<MeterReadingsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<MeterReading> MeterReadings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public static List<Account> GetSeedAccountsTestData()
        {
            var csvPath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData", "Test_Accounts.csv");
            var accounts = new List<Account>();

            using var reader = new StreamReader(csvPath);
            var header = reader.ReadLine(); // Skip header

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length != 3) continue;

                accounts.Add(new Account
                {
                    Id = int.Parse(parts[0]),
                    FirstName = parts[1].Trim(),
                    LastName = parts[2].Trim()
                });
            }

            return accounts;
        }
    }
}
