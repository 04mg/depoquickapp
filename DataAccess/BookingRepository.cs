using Domain;

namespace DataAccess;

public class BookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings = new();
    private int NextBookingId => _bookings.Count > 0 ? _bookings.Max(d => d.Id) + 1 : 1;

    public void Add(Booking booking)
    {
        booking.Id = NextBookingId;
        _bookings.Add(booking);
    }

    public bool Exists(int id)
    {
        return _bookings.Any(b => b.Id == id);
    }

    public Booking Get(int id)
    {
        return _bookings.First(b => b.Id == id);
    }

    public IEnumerable<Booking> GetAll()
    {
        return _bookings;
    }
}