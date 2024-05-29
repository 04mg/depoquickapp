using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Repositories;

namespace BusinessLogic.Services;

public class BookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IDepositRepository _depositRepository;
    private readonly IUserRepository _userRepository;
    private IEnumerable<Booking> AllBookings => _bookingRepository.GetAll();

    public BookingService(IBookingRepository bookingRepository,
        IDepositRepository depositRepository, IUserRepository userRepository)
    {
        _bookingRepository = bookingRepository;
        _depositRepository = depositRepository;
        _userRepository = userRepository;
    }

    public void AddBooking(Booking booking)
    {
        EnsureUserExists(booking.Client.Email);
        EnsureDepositExists(booking.Deposit.Name);
        _bookingRepository.Add(booking);
    }

    private void EnsureDepositExists(string name)
    {
        if (!_depositRepository.Exists(name)) throw new ArgumentException("Deposit not found.");
    }

    private void EnsureUserExists(string email)
    {
        if (!_userRepository.Exists(email)) throw new ArgumentException("User not found.");
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