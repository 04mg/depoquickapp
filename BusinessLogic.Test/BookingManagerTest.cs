namespace BusinessLogic.Test;

[TestClass]
public class BookingManagerTest
{
    private AuthManager _authManager = new();
    private Credentials _adminCredentials;
    private Credentials _userCredentials;
    private Deposit? _deposit;
    private User? _client;
    private User? _admin;
    private User? _otherClient;
    private readonly BookingManager _bookingManager = new();

    [TestInitialize]
    public void Initialize()
    {
        _authManager = new AuthManager();
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
        _authManager.Register(_admin, passwordConfirmation);
        _adminCredentials = _authManager.Login(new LoginDto() { Email = "admin@test.com", Password = "12345678@mE" });
        _authManager.Register(_client, passwordConfirmation);
        _userCredentials = _authManager.Login(new LoginDto() { Email = "test@test.com", Password = "12345678@mE" });
        _authManager.Register(_otherClient, passwordConfirmation);

        var promotionList = new List<Promotion>()
        {
            new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
        };
        _deposit = new Deposit(1, "A", "Small", true, promotionList);
    }

    [TestMethod]
    public void TestCanAddBooking()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        // Act
        _bookingManager.Add(booking);

        // Assert
        Assert.AreEqual(1, _bookingManager.Bookings.Count);
    }

    [TestMethod]
    public void TestCanCheckIfBookingExists()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _bookingManager.Add(booking);

        // Act
        var bookingExists = _bookingManager.Exists(1);

        // Assert
        Assert.IsTrue(bookingExists);
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfAdministrator()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _bookingManager.Add(booking);

        // Act
        var bookings = _bookingManager.GetBookingsByEmail("test@test.com", _adminCredentials);

        // Assert
        Assert.AreSame(bookings[0], booking);
    }

    [TestMethod]
    public void TestCanGetAllBookings()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        var otherBooking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _bookingManager.Add(booking);
        _bookingManager.Add(otherBooking);

        // Act
        var bookings = _bookingManager.GetAllBookings(_adminCredentials);

        // Assert
        Assert.AreEqual(2, bookings.Count);
    }

    [TestMethod]
    public void TestCanApproveBooking()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _bookingManager.Add(booking);

        // Act
        _bookingManager.Manage(1, _adminCredentials, true);

        // Assert
        Assert.AreEqual(BookingStage.Approved, _bookingManager.Bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanRejectBooking()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _bookingManager.Add(booking);

        // Act
        _bookingManager.Manage(1, _adminCredentials, false);

        // Assert
        Assert.AreEqual(BookingStage.Rejected, _bookingManager.Bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanAddMessageToRejection()
    {
        //Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        const string message = "example";
        _bookingManager.Add(booking);

        //Act
        _bookingManager.Manage(1, _adminCredentials, false, message);

        //Assert
        Assert.AreEqual(message, _bookingManager.Bookings[0].Message);
    }

    [TestMethod]
    public void TestCantAddBookingThatAlreadyExists()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        var repeatedBooking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _bookingManager.Add(booking);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingManager.Add(repeatedBooking));

        // Assert
        Assert.AreEqual("User already has a booking for this period.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfNotAdministrator()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _bookingManager.Add(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingManager.GetBookingsByEmail("other@test.com", _userCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfClient()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _bookingManager.Add(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingManager.GetBookingsByEmail("other@test.com", _userCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetAllBookingsIfNotAdministrator()
    {
        // Arrange
        var booking = new Booking(1, _deposit!, _otherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _bookingManager.Add(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingManager.GetAllBookings(_userCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantManageBookingsIfNotAdministrator()
    {
        //Arrange
        var booking = new Booking(1, _deposit!, _client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _bookingManager.Add(booking);

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingManager.Manage(1, _userCredentials, true));
        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);  
    }
}