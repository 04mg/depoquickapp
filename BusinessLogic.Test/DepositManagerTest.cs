using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Managers;

namespace BusinessLogic.Test;

[TestClass]
public class DepositManagerTest
{
    private const string Name = "Deposit";
    private const string Area = "A";
    private const string Size = "Small";
    private const bool ClimateControl = true;
    private Credentials _adminCredentials;
    private Credentials _clientCredentials;
    private DepositManager _depositManager = new();
    private PromotionManager _promotionManager = new();

    [TestInitialize]
    public void Initialize()
    {
        _depositManager = new DepositManager();
        RegisterUsers();
        CreatePromotion();
    }

    private void RegisterUsers()
    {
        var authManager = new AuthManager();

        const string passwordConfirmation = "12345678@mE";
        var admin = new User(
            "Name Surname",
            "admin@admin.com",
            "12345678@mE",
            "Administrator"
        );
        var client = new User(
            "Name Surname",
            "client@client.com",
            "12345678@mE"
        );

        authManager.Register(admin, passwordConfirmation);
        authManager.Register(client, passwordConfirmation);

        _adminCredentials = authManager.Login(new LoginDto { Email = admin.Email, Password = admin.Password });
        _clientCredentials = authManager.Login(new LoginDto { Email = client.Email, Password = client.Password });
    }

    private void CreatePromotion()
    {
        _promotionManager = new PromotionManager();

        var promotion = new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        _promotionManager.Add(promotion, _adminCredentials);
    }

    [TestMethod]
    public void TestCanAddDepositWithValidData()
    {
        // Arrange
        var promotionList = new List<Promotion> { _promotionManager.GetAllPromotions(_adminCredentials)[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);

        // Act
        _depositManager.Add(deposit, _adminCredentials);

        // Assert
        Assert.AreEqual(1, _depositManager.GetAllDeposits().Count);
    }

    [TestMethod]
    public void TestCanDeleteDeposit()
    {
        // Arrange
        var promotionList = new List<Promotion> { _promotionManager.GetAllPromotions(_adminCredentials)[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositManager.Add(deposit, _adminCredentials);

        // Act
        _depositManager.Delete(Name, _adminCredentials);

        // Assert
        Assert.AreEqual(0, _depositManager.GetAllDeposits().Count);
    }

    [TestMethod]
    public void TestCantDeleteNonExistentDeposit()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => _depositManager.Delete(Name, _adminCredentials));

        // Assert
        Assert.AreEqual("Deposit not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantAddDepositIfNotAdministrator()
    {
        // Arrange
        var promotionList = new List<Promotion> { _promotionManager.GetAllPromotions(_adminCredentials)[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() => _depositManager.Add(deposit, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage deposits.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeleteDepositIfNotAdministrator()
    {
        // Arrange
        var promotionList = new List<Promotion> { _promotionManager.GetAllPromotions(_adminCredentials)[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositManager.Add(deposit, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() => _depositManager.Delete(Name, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage deposits.", exception.Message);
    }

    [TestMethod]
    public void TestCanGetAllDeposits()
    {
        // Arrange
        var promotionList = new List<Promotion> { _promotionManager.GetAllPromotions(_adminCredentials)[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositManager.Add(deposit, _adminCredentials);

        // Act
        var deposits = _depositManager.GetAllDeposits();

        // Assert
        Assert.IsNotNull(deposits);
        Assert.AreEqual(1, deposits.Count);
        Assert.AreEqual(Name, deposits[0].Name);
        Assert.AreEqual(Area, deposits[0].Area);
        Assert.AreEqual(Size, deposits[0].Size);
        Assert.AreEqual(ClimateControl, deposits[0].ClimateControl);
        Assert.AreEqual(promotionList, deposits[0].Promotions);
    }
    
    [TestMethod]
    public void TestCantAddDepositIfNameIsAlreadyTaken()
    {
        // Arrange
        var promotionList = new List<Promotion> { _promotionManager.GetAllPromotions(_adminCredentials)[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositManager.Add(deposit, _adminCredentials);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => _depositManager.Add(deposit, _adminCredentials));

        // Assert
        Assert.AreEqual("Deposit name is already taken.", exception.Message);
    }
    
    [TestMethod]
    public void TestCanAddAvailabilityPeriodToDeposit()
    {
        // Arrange
        var promotionList = new List<Promotion> { _promotionManager.GetAllPromotions(_adminCredentials)[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositManager.Add(deposit, _adminCredentials);
        var availabilityPeriod = new DateRange(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        // Act
        _depositManager.AddAvailabilityPeriod(Name, availabilityPeriod, _adminCredentials);

        // Assert
        var deposits = _depositManager.GetAllDeposits();
        Assert.AreEqual(1, deposits.Count);
        Assert.AreEqual(1, deposits[0].AvailabilityPeriods.Count);
        Assert.AreEqual(DateOnly.FromDateTime(DateTime.Now), deposits[0].AvailabilityPeriods[0].StartDate);
        Assert.AreEqual(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), deposits[0].AvailabilityPeriods[0].EndDate);
    }
    
    [TestMethod]
    public void TestCantAddAvailabilityPeriodIfNotAdministrator()
    {
        // Arrange
        var promotionList = new List<Promotion> { _promotionManager.GetAllPromotions(_adminCredentials)[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositManager.Add(deposit, _adminCredentials);
        var availabilityPeriod = new DateRange(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() => _depositManager.AddAvailabilityPeriod(Name, availabilityPeriod, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage deposits.", exception.Message);
    }

    [TestMethod]
    public void TestCantAddAvailabilityPeriodIfDepositDoesNotExist()
    {
        // Arrange
        var availabilityPeriod = new DateRange(DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _depositManager.AddAvailabilityPeriod(Name, availabilityPeriod, _adminCredentials));

        // Assert
        Assert.AreEqual("Deposit not found.", exception.Message);
    }
}