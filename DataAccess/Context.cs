using Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataAccess;

using Microsoft.EntityFrameworkCore;

public class Context : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Promotion> Promotions { get; set; }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var navigation in entityType.GetNavigations())
            {
                navigation.SetIsEagerLoaded(true);
            }
        }

        modelBuilder.Entity<Deposit>()
            .OwnsOne(d => d.AvailabilityPeriods)
            .OwnsMany(a => a.AvailablePeriods, dr =>
            {
                dr.Property<int>("Id");
                dr.HasKey("Id");
            });

        modelBuilder.Entity<Deposit>()
            .OwnsOne(d => d.AvailabilityPeriods)
            .OwnsMany(a => a.UnavailablePeriods, dr =>
            {
                dr.Property<int>("Id");
                dr.HasKey("Id");
            });

        modelBuilder.Entity<Promotion>()
            .OwnsOne(p => p.Validity);

        modelBuilder.Entity<Booking>()
            .OwnsOne(b => b.Duration);
        
        modelBuilder.Entity<Deposit>()
            .HasMany(d => d.Promotions)
            .WithMany(p => p.Deposits)
            .UsingEntity(j => j.ToTable("DepositPromotion"));

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>()
            .HaveColumnType("date");

        base.ConfigureConventions(builder);
    }
}

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter()
        : base(dateOnly =>
                dateOnly.ToDateTime(TimeOnly.MinValue),
            dateTime => DateOnly.FromDateTime(dateTime))
    {
    }
}