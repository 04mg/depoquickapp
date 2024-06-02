using Domain;

namespace DataAccess;

using Microsoft.EntityFrameworkCore;

public class Context : DbContext
{
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<User> Users { get; set; }

    public Context(DbContextOptions<Context> options) : base(options)
    {
        var relationalOptionsExtension = options.Extensions
            .OfType<Microsoft.EntityFrameworkCore.Infrastructure.RelationalOptionsExtension>()
            .FirstOrDefault();

        var databaseType = relationalOptionsExtension?.Connection?.GetType().Name;
        if (databaseType != null && databaseType.Contains("Sqlite"))
            Database.EnsureCreated();
        else
            Database.Migrate();
    }
}