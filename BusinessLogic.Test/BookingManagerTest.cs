namespace BusinessLogic.Test;

[TestClass]
public class BookingManagerTest
{

    private AuthManager _authManager = new();
    private DepositManager _depositManager = new();
    private Credentials _credentials = new();
    private BookingManager bookingManager = new();
    
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
        _credentials = _authManager.Login(new LoginDto() { Email = "admin@test.com", Password = "12345678@mE" });

        var userModel = new RegisterDto()
        {
            NameSurname = "Name Surname",
            Email = "test@test.com",
            Password = "12345678@mE",
            PasswordConfirmation = "12345678@mE",
            Rank = "Client"
        };
        _authManager.Register(userModel);

        var promotionManager = new PromotionManager();
        var promotionModel1 = new AddPromotionDto()
        {
            Label = "label",
            Discount = 50,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        promotionManager.Add(promotionModel1, _credentials);
        var promotionList = new List<int>() { 1 };

        var depositModel = new AddDepositDto()
        {
            Area = "A",
            Size = "Small",
            ClimateControl = true,
            PromotionList = promotionList
        };
        _depositManager.Add(depositModel, _credentials, promotionManager);
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
        bookingManager.Add(addBookingDto, _depositManager, _authManager);
        
        // Assert
        Assert.AreEqual(1, bookingManager.Bookings.Count);
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
        bookingManager.Add(addBookingDto, _depositManager, _authManager);
        
        //Act
        var bookingExists = bookingManager.Exists(1);
        
        //Assert
        Assert.IsTrue(bookingExists);        
    }

    [TestMethod]
    public void TestCanGetBookingsByEmail()
    {
        var addBookingDto = new AddBookingDto()
        {
            DepositId = 1,
            Email = "admin@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        bookingManager.Add(addBookingDto, _depositManager, _authManager);

        //Act
        var bookings = bookingManager.GetBookingsByEmail("admin@test.com", _credentials);

        //Assert
        Assert.AreEqual(1, bookings.Count);
    }
}