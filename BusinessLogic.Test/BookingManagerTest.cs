namespace BusinessLogic.Test;

[TestClass]
public class BookingManagerTest
{
    private AuthManager _authManager = new();
    private readonly DepositManager _depositManager = new();
    private Credentials _adminCredentials;
    private Credentials _userCredentials;
    private BookingManager _bookingManager = new();

    [TestInitialize]
    public void Initialize()
    {
        _authManager = new AuthManager();
        var adminModel = new RegisterDto()
        {
            NameSurname = "Name Surname",
            Email = "admin@test.com",
            Password = "12345678@mE",
            PasswordConfirmation = "12345678@mE",
            Rank = "Administrator"
        };
        _authManager.Register(adminModel);
        _adminCredentials = _authManager.Login(new LoginDto() { Email = "admin@test.com", Password = "12345678@mE" });

        var userModel = new RegisterDto()
        {
            NameSurname = "Name Surname",
            Email = "test@test.com",
            Password = "12345678@mE",
            PasswordConfirmation = "12345678@mE",
            Rank = "Client"
        };
        _authManager.Register(userModel);
        _userCredentials = _authManager.Login(new LoginDto() { Email = "test@test.com", Password = "12345678@mE" });

        var otherUserModel = new RegisterDto()
        {
            NameSurname = "Name Surname",
            Email = "other@test.com",
            Password = "12345678@mE",
            PasswordConfirmation = "12345678@mE",
            Rank = "Client"
        };
        _authManager.Register(otherUserModel);

        var promotionManager = new PromotionManager();
        var promotionModel1 = new AddPromotionDto()
        {
            Label = "label",
            Discount = 50,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        promotionManager.Add(promotionModel1, _adminCredentials);
        var promotionList = new List<int>() { 1 };

        var depositModel = new AddDepositDto()
        {
            Area = "A",
            Size = "Small",
            ClimateControl = true,
            PromotionList = promotionList
        };
        _depositManager.Add(depositModel, _adminCredentials, promotionManager);
    }

    [TestMethod]
    public void TestCanAddBooking()
    {
        // Arrange
        var addBookingDto = new AddBookingDto()
        {
            DepositId = 1,
            Email = "test@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        // Act
        _bookingManager.Add(addBookingDto, _depositManager, _authManager);

        // Assert
        Assert.AreEqual(1, _bookingManager.Bookings.Count);
    }

    [TestMethod]
    public void TestCanCheckIfBookingExists()
    {
        var addBookingDto = new AddBookingDto()
        {
            DepositId = 1,
            Email = "test@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        _bookingManager.Add(addBookingDto, _depositManager, _authManager);

        //Act
        var bookingExists = _bookingManager.Exists(1);

        //Assert
        Assert.IsTrue(bookingExists);
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfAdministrator()
    {
        var addBookingDto = new AddBookingDto()
        {
            DepositId = 1,
            Email = "admin@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        _bookingManager.Add(addBookingDto, _depositManager, _authManager);

        //Act
        var bookings = _bookingManager.GetBookingsByEmail("admin@test.com", _adminCredentials);

        //Assert
        Assert.AreEqual(bookings[0].Id, 1);
        Assert.AreEqual(bookings[0].Email, "admin@test.com");
        Assert.AreEqual(bookings[0].DepositId, 1);
        Assert.AreEqual(bookings[0].Duration.Item1, DateOnly.FromDateTime(DateTime.Now));
        Assert.AreEqual(bookings[0].Duration.Item2, DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
    }

    [TestMethod]
    public void TestCanGetAllBookings()
    {
        var addBookingDto = new AddBookingDto()
        {
            DepositId = 1,
            Email = "admin@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        var addBookingDto2 = new AddBookingDto()
        {
            DepositId = 1,
            Email = "test@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        _bookingManager.Add(addBookingDto, _depositManager, _authManager);
        _bookingManager.Add(addBookingDto2, _depositManager, _authManager);

        //Act
        var bookings = _bookingManager.GetAllBookings(_adminCredentials);

        //Assert
        Assert.AreEqual(2, bookings.Count);
    }

    [TestMethod]
    public void TestCanApproveBooking()
    {
        //Arrange
        var addBookingDto = new AddBookingDto()
        {
            DepositId = 1,
            Email = "test@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        _bookingManager.Add(addBookingDto, _depositManager, _authManager);

        //Act
        _bookingManager.Manage(1, _adminCredentials, true);

        //Assert
        Assert.AreEqual(BookingStage.Approved, _bookingManager.Bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanRejectBooking()
    {
        //Arrange
        var addBookingDto = new AddBookingDto()
        {
            DepositId = 1,
            Email = "test@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        _bookingManager.Add(addBookingDto, _depositManager, _authManager);

        //Act
        _bookingManager.Manage(1, _adminCredentials, false);

        //Assert
        Assert.AreEqual(BookingStage.Rejected, _bookingManager.Bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanAddMessageToRejection()
    {
        //Arrange
        var addBookingDto = new AddBookingDto()
        {
            DepositId = 1,
            Email = "test@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        var message = "example";
        _bookingManager.Add(addBookingDto, _depositManager, _authManager);

        //Act
        _bookingManager.Manage(1, _adminCredentials, false, message);

        //Assert
        Assert.AreEqual(message, _bookingManager.Bookings[0].Message);
    }

    [TestMethod]
    public void TestCantAddBookingThatAlreadyExists()
    {
        // Arrange
        var firstBooking = new AddBookingDto()
        {
            DepositId = 1,
            Email = "test@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        var repeatedBooking = new AddBookingDto()
        {
            DepositId = 1,
            Email = "test@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
        };
        _bookingManager.Add(firstBooking, _depositManager, _authManager);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingManager.Add(repeatedBooking, _depositManager, _authManager));

        // Assert
        Assert.AreEqual("User already has a booking for this period.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfNotAdministrator()
    {
        // Arrange
        var addBookingDto = new AddBookingDto()
        {
            DepositId = 1,
            Email = "other@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        _bookingManager.Add(addBookingDto, _depositManager, _authManager);

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
        var addBookingDto = new AddBookingDto()
        {
            DepositId = 1,
            Email = "other@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        _bookingManager.Add(addBookingDto, _depositManager, _authManager);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingManager.GetBookingsByEmail("other@test.com", _userCredentials));
        
        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }
}