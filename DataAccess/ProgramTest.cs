using DataAccess.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public class ProgramTest
{
    public readonly ServiceProvider ServiceProvider;

    public ProgramTest()
    {
        var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
        inMemorySqlite.Open();
        var services = new ServiceCollection();
        services.AddDbContextFactory<Context>(options =>
            options.UseSqlite(inMemorySqlite));
        services.AddScoped<UserRepository>();
        services.AddScoped<DepositRepository>();
        services.AddScoped<BookingRepository>();
        services.AddScoped<PromotionRepository>();
        ServiceProvider = services.BuildServiceProvider();
    }
}