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
        context.Attach(booking.Deposit);
        context.Attach(booking.Client);
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
        return context.Bookings.Include(b => b.Deposit).ThenInclude(d => d.Promotions).ToList();
    }

    public void Update(Booking booking)
    {
        using var context = _contextFactory.CreateDbContext();
        var existingBooking = context.Bookings.Include(b => b.Payment).First(b => b.Id == booking.Id);
        context.Entry(existingBooking).CurrentValues.SetValues(booking);
        if (booking.Payment == null)
        {
            if (existingBooking.Payment != null)
            {
                context.Remove(existingBooking.Payment);
            }
        }
        else
        {
            if (existingBooking.Payment == null)
            {
                context.Add(booking.Payment);
            }
            else
            {
                context.Entry(existingBooking.Payment).CurrentValues.SetValues(booking.Payment);
            }
        }
        context.Bookings.Update(existingBooking);
        context.SaveChanges();
    }
}