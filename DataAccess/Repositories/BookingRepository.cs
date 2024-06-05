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
        return context.Bookings.Include(b => b.Payment).Include(b => b.Deposit).Include(b => b.Client)
            .First(b => b.Id == id);
    }

    public IEnumerable<Booking> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Bookings.Include(b => b.Payment).Include(b => b.Client).Include(b => b.Deposit)
            .ThenInclude(d => d.Promotions).ToList();
    }

    public void Update(Booking booking)
    {
        using var context = _contextFactory.CreateDbContext();
        var existingBooking = Get(booking.Id);
        context.Entry(existingBooking).CurrentValues.SetValues(booking);
        HandlePaymentUpdate(booking, existingBooking, context);
        context.Bookings.Update(existingBooking);
        context.SaveChanges();
    }

    private static void HandlePaymentUpdate(Booking booking, Booking existingBooking, Context context)
    {
        if (booking.Payment == null)
        {
            RemoveExistingPayment(existingBooking, context);
        }
        else
        {
            AddOrUpdatePayment(booking, existingBooking, context);
        }
    }

    private static void AddOrUpdatePayment(Booking booking, Booking existingBooking, DbContext context)
    {
        if (booking.Payment == null) return;
        if (existingBooking.Payment == null)
        {
            context.Add(booking.Payment);
        }
        else
        {
            context.Entry(existingBooking.Payment).CurrentValues.SetValues(booking.Payment);
        }
    }

    private static void RemoveExistingPayment(Booking existingBooking, Context context)
    {
        if (existingBooking.Payment != null)
        {
            context.Remove(existingBooking.Payment);
        }
    }
}