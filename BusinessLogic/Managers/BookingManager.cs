using BusinessLogic.Domain;
using BusinessLogic.DTOs;

namespace BusinessLogic.Managers;

public class BookingManager
{
    private List<Booking> Bookings { get; } = new();

    public void Add(Booking booking)
    {
        EnsureNoOverlappingBooking(booking);
        booking.Id = NextBookingId;
        Bookings.Add(booking);
    }

    private void EnsureNoOverlappingBooking(Booking booking)
    {
        GetBookingsByUserAndDeposit(booking.Client.Email, booking.Deposit.Id)
            .ForEach(b => EnsureNoOverlappingDates(booking, b));
    }

    private List<Booking> GetBookingsByUserAndDeposit(string email, int depositId)
    {
        return Bookings.Where(b => b.Client.Email == email && b.Deposit.Id == depositId).ToList();
    }

    private static void EnsureNoOverlappingDates(Booking booking, Booking anotherBooking)
    {
        if (BookingEndDateOverlaps(booking, anotherBooking) || BookingStartDateOverlaps(booking, anotherBooking))
        {
            throw new ArgumentException("User already has a booking for this period.");
        }
    }

    private static bool BookingStartDateOverlaps(Booking booking, Booking anotherBooking)
    {
        return booking.Duration.Item1 >= anotherBooking.Duration.Item1 &&
               booking.Duration.Item1 <= anotherBooking.Duration.Item2;
    }

    private static bool BookingEndDateOverlaps(Booking booking, Booking anotherBooking)
    {
        return booking.Duration.Item2 >= anotherBooking.Duration.Item1 &&
               booking.Duration.Item2 <= anotherBooking.Duration.Item2;
    }

    private int NextBookingId => Bookings.Count > 0 ? Bookings.Max(d => d.Id) + 1 : 1;

    public bool Exists(int id)
    {
        return Bookings.Any(b => b.Id == id);
    }

    public List<Booking> GetBookingsByEmail(string email, Credentials credentials)
    {
        EnsureUserIsAdministratorOrEmailMatches(email, credentials);
        return Bookings.Where(b => b.Client.Email == email).ToList();
    }

    private static void EnsureUserIsAdministrator(Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
        {
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
        }
    }

    private static void EnsureUserIsAdministratorOrEmailMatches(string email, Credentials credentials)
    {
        if (credentials.Rank != "Administrator" && credentials.Email != email)
        {
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
        }
    }

    public void EnsureThereAreNoBookingsWithThisDeposit(int id)
    {
        if (Bookings.Any(booking => booking.Deposit.Id == id))
        {
            throw new ArgumentException("There are existing bookings for this deposit.");
        }
    }

    public List<Booking> GetAllBookings(Credentials credentials)
    {
        EnsureUserIsAdministrator(credentials);
        return Bookings;
    }

    public void Approve(int id, Credentials credentials)
    {
        EnsureUserIsAdministrator(credentials);
        var booking = GetBookingById(id);
        booking.Approve();
    }

    public void Reject(int id, Credentials credentials, string message = "")
    {
        EnsureMessageIsNotEmpty(message);
        EnsureUserIsAdministrator(credentials);
        var booking = GetBookingById(id);
        booking.Reject(message);
    }

    private void EnsureMessageIsNotEmpty(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be empty.");
        }
    }

    public Booking GetBookingById(int id)
    {
        EnsureBookingExists(id);
        return Bookings.First(b => b.Id == id);
    }

    private void EnsureBookingExists(int id)
    {
        if (Bookings.All(b => b.Id != id))
        {
            throw new ArgumentException("Booking not found.");
        }
    }
}