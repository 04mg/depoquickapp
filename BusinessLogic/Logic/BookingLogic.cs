using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Repositories;

namespace BusinessLogic.Logic;

public class BookingLogic
{
    private readonly IBookingRepository _bookingRepository;
    private IEnumerable<Booking> AllBookings => _bookingRepository.GetAll();

    public BookingLogic()
    {
        _bookingRepository = new BookingRepository();
    }

    public void AddBooking(Booking booking)
    {
        EnsureNoOverlappingBooking(booking);
        _bookingRepository.Add(booking);
    }

    private void EnsureNoOverlappingBooking(Booking booking)
    {
        GetBookingsByUserAndDeposit(booking.Client.Email, booking.Deposit.Name)
            .ForEach(b => EnsureNoOverlappingDates(booking, b));
    }

    private List<Booking> GetBookingsByUserAndDeposit(string email, string depositName)
    {
        depositName = depositName.ToLower();
        return AllBookings
            .Where(b => b.Client.Email == email && b.Deposit.Name.ToLower() == depositName).ToList();
    }

    private static void EnsureNoOverlappingDates(Booking booking, Booking anotherBooking)
    {
        var overlaps = BookingEndDateOverlaps(booking, anotherBooking);
        overlaps |= BookingStartDateOverlaps(booking, anotherBooking);
        if (overlaps) throw new ArgumentException("User already has a booking for this period.");
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

    public List<Booking> GetBookingsByEmail(string email, Credentials credentials)
    {
        EnsureUserIsAdministratorOrEmailMatches(email, credentials);
        return AllBookings.Where(b => b.Client.Email == email).ToList();
    }

    private static void EnsureUserIsAdministrator(Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
    }

    private static void EnsureUserIsAdministratorOrEmailMatches(string email, Credentials credentials)
    {
        if (credentials.Rank != "Administrator" && credentials.Email != email)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
    }

    public IEnumerable<Booking> GetAllBookings(Credentials credentials)
    {
        EnsureUserIsAdministrator(credentials);
        return AllBookings;
    }

    public void ApproveBooking(int id, Credentials credentials)
    {
        EnsureUserIsAdministrator(credentials);
        var booking = GetBooking(id);
        booking.Approve();
    }

    public void RejectBooking(int id, Credentials credentials, string message = "")
    {
        EnsureMessageIsNotEmpty(message);
        EnsureUserIsAdministrator(credentials);
        var booking = GetBooking(id);
        booking.Reject(message);
    }

    private static void EnsureMessageIsNotEmpty(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message cannot be empty.");
    }

    public void EnsureThereAreNoBookingsWithThisDeposit(string depositName)
    {
        if (AllBookings.Any(b => b.Deposit.Name == depositName))
            throw new ArgumentException("There are existing bookings for this deposit.");
    }

    public Booking GetBooking(int id)
    {
        EnsureBookingExists(id);
        return _bookingRepository.Get(id);
    }

    private void EnsureBookingExists(int id)
    {
        if (!_bookingRepository.Exists(id)) throw new ArgumentException("Booking not found.");
    }
}