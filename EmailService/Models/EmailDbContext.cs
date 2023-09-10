
using EmailService.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents the database context for the EmailService.
/// </summary>
public class EmailDbContext : DbContext
{
    public DbSet<EmailAccount> EmailAccounts { get; set; }
    public DbSet<EmailMessage> EmailMessages { get; set; }

    public EmailDbContext(DbContextOptions<EmailDbContext> options)
        : base(options)
    {
    }
    
    /// <summary>
    /// Ensures the database is created and seeded with mock data if needed.
    /// </summary>
    public void EnsureDatabaseAndSeedData()
    {
        // Ensure the database is created
        this.Database.EnsureCreated();
        
        // Seed mock data if no email accounts exist
        if (!EmailAccounts.Any()) 
        {
            EmailAccounts.AddRange(
                new EmailAccount { Address = "mock1@example.com" },
                new EmailAccount { Address = "mock2@example.com" }
                //... Add more mock email accounts as needed
            );
            SaveChanges();
        }
    }
}