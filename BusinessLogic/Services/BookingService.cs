using BusinessLogic.DTOs;
using Calculator;
using DataAccess.Repositories;
using Domain;
using ReportGenerator;

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

    public void AddBooking(BookingDto bookingDto, Credentials credentials)
    {
        EnsureUserExists(bookingDto.Email);
        EnsureEmailMatches(bookingDto.Email, credentials);
        EnsureDepositExists(bookingDto.DepositName);
        _bookingRepository.Add(BookingFromDto(bookingDto));
    }

    private Booking BookingFromDto(BookingDto bookingDto)
    {
        
        return new Booking(bookingDto.Id, _depositRepository.Get(bookingDto.DepositName),
            _userRepository.Get(bookingDto.Email),
            bookingDto.DateFrom, bookingDto.DateTo, new Payment(CalculateBookingPrice(bookingDto)));
    }

    private void EnsureDepositExists(string name)
    {
        if (!_depositRepository.Exists(name)) throw new ArgumentException("Deposit not found.");
    }

    private void EnsureUserExists(string email)
    {
        if (!_userRepository.Exists(email)) throw new ArgumentException("User not found.");
    }

    public IEnumerable<BookingDto> GetBookingsByEmail(string email, Credentials credentials)
    {
        EnsureUserIsAdministratorOrEmailMatches(email, credentials);
        return AllBookings.Where(b => b.Client.Email == email).Select(BookingDtoFromBooking);
    }

    private static BookingDto BookingDtoFromBooking(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            DateFrom = booking.Duration.StartDate,
            DateTo = booking.Duration.EndDate,
            DepositName = booking.Deposit.Name,
            Email = booking.Client.Email,
            Stage = booking.Stage.ToString(),
            Message = booking.Message,
            Payment = PaymentDtoFromPayment(booking.Payment)
        };
    }

    private static PaymentDto? PaymentDtoFromPayment(IPayment? payment)
    {
        if (payment == null) return null;
        return new PaymentDto
        {
            Amount = payment.GetAmount(),
            Captured = payment.IsCaptured()
        };
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

    private static void EnsureEmailMatches(string email, Credentials credentials)
    {
        if (credentials.Email != email)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
    }

    public IEnumerable<BookingDto> GetAllBookings(Credentials credentials)
    {
        EnsureUserIsAdministrator(credentials);
        return AllBookings.Select(BookingDtoFromBooking);
    }

    public void ApproveBooking(int id, Credentials credentials)
    {
        EnsureUserIsAdministrator(credentials);
        EnsureBookingExists(id);
        var booking = _bookingRepository.Get(id);
        booking.Approve();
        _bookingRepository.Update(booking);
    }

    public void RejectBooking(BookingDto bookingDto, Credentials credentials)
    {
        EnsureMessageIsNotEmpty(bookingDto.Message);
        EnsureUserIsAdministrator(credentials);
        EnsureBookingExists(bookingDto.Id);
        var booking = _bookingRepository.Get(bookingDto.Id);
        booking.Reject(bookingDto.Message);
        _bookingRepository.Update(booking);
    }

    private static void EnsureMessageIsNotEmpty(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message cannot be empty.");
    }

    public BookingDto GetBooking(int id, Credentials credentials)
    {
        EnsureBookingExists(id);
        var booking = _bookingRepository.Get(id);
        EnsureUserIsAdministratorOrEmailMatches(booking.Client.Email, credentials);
        return BookingDtoFromBooking(booking);
    }

    private void EnsureBookingExists(int id)
    {
        if (!_bookingRepository.Exists(id)) throw new ArgumentException("Booking not found.");
    }

    public void GenerateReport(string type, Credentials credentials)
    {
        EnsureUserIsAdministrator(credentials);
        switch (type)
        {
            case "txt":
                new BookingReportGenerator(new TxtBookingReport()).GenerateReport(_bookingRepository.GetAll());
                break;
            case "csv":
                new BookingReportGenerator(new CsvBookingReport()).GenerateReport(_bookingRepository.GetAll());
                break;
            default:
                throw new ArgumentException("Invalid format. Supported formats: txt, csv.");
        }
    }

    public double CalculateBookingPrice(BookingDto bookingDto)
    {
        EnsureDepositExists(bookingDto.DepositName);
        var priceCalculator = new PriceCalculator();
        var deposit = _depositRepository.Get(bookingDto.DepositName);
        return priceCalculator.CalculatePrice(deposit, bookingDto.DateFrom, bookingDto.DateTo);
    }
}