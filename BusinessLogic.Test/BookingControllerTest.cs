using BusinessLogic.Calculators;
using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Enums;
using BusinessLogic.Managers;

namespace BusinessLogic.Test;

[TestClass]
public class BookingControllerTest
{
    private readonly BookingController _bookingController = new();
    private User? _admin;
    private Credentials _adminCredentials;
    private AuthController _authController = new();
    private User? _client;
    private Deposit? _deposit;
    private User? _otherClient;
    private Credentials _userCredentials;

    [TestInitialize]
    public void Initialize()
    {
        RegisterUsers();
        CreateDeposit();
    }

    private void RegisterUsers()
    {
        _authController = new AuthController();
        const string passwordConfirmation = "12345678@mE";

        _admin = new User(
            "Name Surname",
            "admin@test.com",
            "12345678@mE",
            "Administrator"
        );

        _client = new User(
            "Name Surname",
            "test@test.com",
            "12345678@mE"
        );

        _otherClient = new User(
            "Name Surname",
            "other@test.com",
            "12345678@mE"
        );

        _authController.Register(_admin, passwordConfirmation);
        _adminCredentials = _authController.Login(new LoginDto { Email = "admin@test.com", Password = "12345678@mE" });
        _authController.Register(_client, passwordConfirmation);
        _userCredentials = _authController.Login(new LoginDto { Email = "test@test.com", Password = "12345678@mE" });
        _authController.Register(_otherClient, passwordConfirmation);
    }

    private void CreateDeposit()
    {
        var promotionList = new List<Promotion>
        {
            new(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
        };

        _deposit = new Deposit("Deposit", "A", "Small", true, promotionList);
    }

    [TestMethod]
    public void TestCanAddBooking()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());

        // Act
        _bookingController.AddBooking(booking);

        // Assert
        Assert.AreEqual(1, _bookingController.GetAllBookings(_adminCredentials).Count());
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfAdministrator()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);

        // Act
        var bookings = _bookingController.GetBookingsByEmail("test@test.com", _adminCredentials);

        // Assert
        Assert.AreSame(bookings[0], booking);
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfSameEmail()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);

        // Act
        var bookings = _bookingController.GetBookingsByEmail("test@test.com", _userCredentials);

        // Assert
        Assert.AreSame(bookings[0], booking);
    }

    [TestMethod]
    public void TestCanGetAllBookings()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        var otherBooking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);
        _bookingController.AddBooking(otherBooking);

        // Act
        var bookings = _bookingController.GetAllBookings(_adminCredentials);

        // Assert
        Assert.AreEqual(2, bookings.Count());
    }

    [TestMethod]
    public void TestCanApproveBooking()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);

        // Act
        _bookingController.ApproveBooking(1, _adminCredentials);
        var bookings = _bookingController.GetAllBookings(_adminCredentials).ToList();

        // Assert
        Assert.AreEqual(BookingStage.Approved, bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanRejectBooking()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);

        // Act
        _bookingController.RejectBooking(1, _adminCredentials, "Message");
        var bookings = _bookingController.GetAllBookings(_adminCredentials).ToList();

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
        _bookingController.AddBooking(booking);

        //Act
        _bookingController.RejectBooking(1, _adminCredentials, message);
        var bookings = _bookingController.GetAllBookings(_adminCredentials).ToList();

        //Assert
        Assert.AreEqual(message, bookings[0].Message);
    }

    [TestMethod]
    public void TestCantAddBookingThatAlreadyExists()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        var repeatedBooking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingController.AddBooking(repeatedBooking));

        // Assert
        Assert.AreEqual("User already has a booking for this period.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfNotAdministrator()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingController.GetBookingsByEmail("other@test.com", _userCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfClient()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingController.GetBookingsByEmail("other@test.com", _userCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetAllBookingsIfNotAdministrator()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingController.GetAllBookings(_userCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantApproveBookingsIfNotAdministrator()
    {
        //Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingController.ApproveBooking(1, _userCredentials));
        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    public void TestCantRejectBookingsIfNotAdministrator()
    {
        //Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingController.RejectBooking(1, _userCredentials, "message"));
        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantApproveNonExistentBooking()
    {
        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingController.ApproveBooking(1, _adminCredentials));
        //Assert
        Assert.AreEqual("Booking not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantRejectNonExistentBooking()
    {
        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingController.RejectBooking(1, _adminCredentials, "message"));
        //Assert
        Assert.AreEqual("Booking not found.", exception.Message);
    }

    [TestMethod]
    public void TestRejectMessageCannotBeEmpty()
    {
        //Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingController.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingController.RejectBooking(1, _adminCredentials));
        //Assert
        Assert.AreEqual("Message cannot be empty.", exception.Message);
    }
}