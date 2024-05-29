using BusinessLogic.Calculators;
using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Enums;
using BusinessLogic.Repositories;
using BusinessLogic.Services;

namespace BusinessLogic.Test;

[TestClass]
public class BookingServiceTest
{
    private User? _admin;
    private Credentials _adminCredentials;
    private User? _client;
    private Deposit? _deposit;
    private User? _otherClient;
    private Credentials _userCredentials;
    private BookingRepository _bookingRepository = new();
    private DepositRepository _depositRepository = new();
    private UserRepository _userRepository = new();

    private BookingService _bookingService =
        new(new BookingRepository(), new DepositRepository(), new UserRepository());

    [TestInitialize]
    public void Initialize()
    {
        InitializeBookingService();
        RegisterUsers();
        CreateDeposit();
    }

    private void InitializeBookingService()
    {
        _bookingRepository = new BookingRepository();
        _depositRepository = new DepositRepository();
        _userRepository = new UserRepository();
        _bookingService = new BookingService(_bookingRepository, _depositRepository, _userRepository);
    }

    private void RegisterUsers()
    {
        _admin = new User(
            "Name Surname",
            "admin@test.com",
            "12345678@mE",
            "Administrator"
        );
        _adminCredentials = new Credentials() { Email = "admin@test.com", Rank = "Administrator" };

        _client = new User(
            "Name Surname",
            "test@test.com",
            "12345678@mE"
        );
        _userCredentials = new Credentials() { Email = "test@test.com", Rank = "Client" };

        _otherClient = new User(
            "Name Surname",
            "other@test.com",
            "12345678@mE"
        );

        _userRepository.Add(_admin);
        _userRepository.Add(_client);
        _userRepository.Add(_otherClient);
    }

    private void CreateDeposit()
    {
        var promotion = new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        var promotionList = new List<Promotion> { promotion };

        _deposit = new Deposit("Deposit", "A", "Small", true, promotionList);
        _deposit.AddAvailabilityPeriod(new DateRange(DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(100))));
        _depositRepository.Add(_deposit);
    }

    [TestMethod]
    public void TestCanAddBooking()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());

        // Act
        _bookingService.AddBooking(booking);

        // Assert
        Assert.AreEqual(1, _bookingService.GetAllBookings(_adminCredentials).Count());
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfAdministrator()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingService.AddBooking(booking);

        // Act
        var bookings = _bookingService.GetBookingsByEmail("test@test.com", _adminCredentials);

        // Assert
        Assert.AreSame(bookings[0], booking);
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfSameEmail()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingService.AddBooking(booking);

        // Act
        var bookings = _bookingService.GetBookingsByEmail("test@test.com", _userCredentials);

        // Assert
        Assert.AreSame(bookings[0], booking);
    }

    [TestMethod]
    public void TestCanGetAllBookings()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        var otherBooking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(3)), new PriceCalculator());
        _bookingService.AddBooking(booking);
        _bookingService.AddBooking(otherBooking);

        // Act
        var bookings = _bookingService.GetAllBookings(_adminCredentials);

        // Assert
        Assert.AreEqual(2, bookings.Count());
    }

    [TestMethod]
    public void TestCanApproveBooking()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingService.AddBooking(booking);

        // Act
        _bookingService.ApproveBooking(1, _adminCredentials);
        var bookings = _bookingService.GetAllBookings(_adminCredentials).ToList();

        // Assert
        Assert.AreEqual(BookingStage.Approved, bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanRejectBooking()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingService.AddBooking(booking);

        // Act
        _bookingService.RejectBooking(1, _adminCredentials, "Message");
        var bookings = _bookingService.GetAllBookings(_adminCredentials).ToList();

        // Assert
        Assert.AreEqual(BookingStage.Rejected, bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanAddMessageToRejection()
    {
        //Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        const string message = "example";
        _bookingService.AddBooking(booking);

        //Act
        _bookingService.RejectBooking(1, _adminCredentials, message);
        var bookings = _bookingService.GetAllBookings(_adminCredentials).ToList();

        //Assert
        Assert.AreEqual(message, bookings[0].Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfNotAdministrator()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingService.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingService.GetBookingsByEmail("other@test.com", _userCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfClient()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingService.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingService.GetBookingsByEmail("other@test.com", _userCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetAllBookingsIfNotAdministrator()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingService.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingService.GetAllBookings(_userCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantApproveBookingsIfNotAdministrator()
    {
        //Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingService.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingService.ApproveBooking(1, _userCredentials));
        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantRejectBookingsIfNotAdministrator()
    {
        //Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingService.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingService.RejectBooking(1, _userCredentials, "message"));
        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantApproveNonExistentBooking()
    {
        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingService.ApproveBooking(1, _adminCredentials));
        //Assert
        Assert.AreEqual("Booking not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantRejectNonExistentBooking()
    {
        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingService.RejectBooking(1, _adminCredentials, "message"));
        //Assert
        Assert.AreEqual("Booking not found.", exception.Message);
    }

    [TestMethod]
    public void TestRejectMessageCannotBeEmpty()
    {
        //Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingService.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingService.RejectBooking(1, _adminCredentials));
        //Assert
        Assert.AreEqual("Message cannot be empty.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingIfDepositDoesNotExist()
    {
        // Arrange
        var deposit = new Deposit("Deposit Two", "A", "Small", true, new List<Promotion>());
        var dateRange = new DateRange(DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        deposit.AddAvailabilityPeriod(dateRange);
        var booking = new Booking(1, deposit, _client!,
            DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { _bookingService.AddBooking(booking); });

        // Assert
        Assert.AreEqual("Deposit not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingIfUserDoesNotExist()
    {
        // Arrange
        var nonExistentUser = new User("Name Surname", "nonexistent@test.com", "12345678@mE");
        var booking = new Booking(1, _deposit!, nonExistentUser, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { _bookingService.AddBooking(booking); });

        // Assert
        Assert.AreEqual("User not found.", exception.Message);
    }
}