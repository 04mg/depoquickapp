using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly IDbContextFactory<Context> _contextFactory;

    public BookingRepository(IDbContextFactory<Context> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public void Add(Booking booking)
    {
        using var context = _contextFactory.CreateDbContext();
        context.Bookings.Add(booking);
        context.SaveChanges();
    }

    public bool Exists(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Bookings.Any(b => b.Id == id);
    }

    public Booking Get(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Bookings.First(b => b.Id == id);
    }

    public IEnumerable<Booking> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Bookings.ToList();
    }
}