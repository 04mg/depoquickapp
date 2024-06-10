using DataAccess.Exceptions;
using DataAccess.Interfaces;
using Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class 
    BookingRepository : IBookingRepository
{
    private readonly IDbContextFactory<Context> _contextFactory;

    public BookingRepository(IDbContextFactory<Context> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public void Add(Booking booking)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            context.Attach(booking.Deposit);
            context.Attach(booking.Client);
            context.Bookings.Add(booking);
            context.SaveChanges();
        }
        catch (SqlException)
        {
            throw new DataAccessException("Connection error, please try again later");
        }
        catch (DbUpdateException)
        {
            throw new DataAccessException("Changes could not be saved");
        }
    }

    public bool Exists(int id)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Bookings.Any(b => b.Id == id);
        }
        catch (SqlException)
        {
            throw new DataAccessException("Connection error, please try again later");
        }
    }

    public Booking Get(int id)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Bookings.Include(b => b.Payment).Include(b => b.Deposit).Include(b => b.Client)
                .First(b => b.Id == id);
        }
        catch (SqlException)
        {
            throw new DataAccessException("Connection error, please try again later");
        }
    }

    public IEnumerable<Booking> GetAll()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Bookings.Include(b => b.Payment).Include(b => b.Client).Include(b => b.Deposit)
                .ThenInclude(d => d.Promotions).ToList();
        }
        catch (SqlException)
        {
            throw new DataAccessException("Connection error, please try again later");
        }
    }

    public void Update(Booking booking)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var existingBooking = Get(booking.Id);
            context.Entry(existingBooking).CurrentValues.SetValues(booking);
            HandlePaymentUpdate(booking, existingBooking, context);
            context.Bookings.Update(existingBooking);
            context.SaveChanges();
        }
        catch (SqlException)
        {
            throw new DataAccessException("Connection error, please try again later");
        }
        catch (DbUpdateException)
        {
            throw new DataAccessException("Changes could not be saved");
        }
    }

    private static void HandlePaymentUpdate(Booking booking, Booking existingBooking, Context context)
    {
        if (booking.Payment == null)
            RemoveExistingPayment(existingBooking, context);
        else
            AddOrUpdatePayment(booking, existingBooking, context);
    }

    private static void AddOrUpdatePayment(Booking booking, Booking existingBooking, DbContext context)
    {
        if (booking.Payment == null) return;
        if (existingBooking.Payment == null)
            context.Add(booking.Payment);
        else
            context.Entry(existingBooking.Payment).CurrentValues.SetValues(booking.Payment);
    }

    private static void RemoveExistingPayment(Booking existingBooking, Context context)
    {
        if (existingBooking.Payment != null) context.Remove(existingBooking.Payment);
    }
}