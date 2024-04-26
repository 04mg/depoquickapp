namespace BusinessLogic.Test;

[TestClass]
public class BookingTest
{
    private const string Email = "test@test.com";
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);
    private readonly DateOnly _tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
    private AuthManager _authManager = new();
    private DepositManager _depositManager = new();
    private Credentials _credentials;

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
            Email = Email,
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
        var promotionList = new List<Promotion>() { promotionManager.Promotions[0] };
        
        var deposit = new Deposit(1, "A", "Small", true, promotionList);
        _depositManager.Add(deposit, _credentials);
    }

    [TestMethod]
    public void TestCanCreateBookingWithValidData()
    {
        // Act
        var booking = new Booking(1, 1, Email, _today, _tomorrow, _depositManager, _authManager);

        // Assert
        Assert.IsNotNull(booking);
    }

    [TestMethod]
    public void TestCantCreateBookingWithDateFromGreaterThanDateTo()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(
                () => new Booking(1, 1, Email, _tomorrow, _today, _depositManager, _authManager));

        // Assert
        Assert.AreEqual("The starting date of the booking must not be later than the ending date.",
            exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingWithDateFromLessThanToday()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                new Booking(1, 1, Email, _today.AddDays(-1), _tomorrow, _depositManager, _authManager));

        // Assert
        Assert.AreEqual("The starting date of the booking must not be earlier than today.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingWithNonExistentDeposit()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(
                () => new Booking(1, 2, Email, _today, _tomorrow, _depositManager, _authManager));

        // Assert
        Assert.AreEqual("The deposit does not exist.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingWithNonExistentUser()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                new Booking(1, 1, "wrong@test.com", _today, _tomorrow, _depositManager, _authManager));

        // Assert
        Assert.AreEqual("The user does not exist.", exception.Message);
    }
}