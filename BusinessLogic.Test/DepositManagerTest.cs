using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Managers;

namespace BusinessLogic.Test;

[TestClass]
public class DepositManagerTest
{
    private const string Area = "A";
    private const string Size = "Small";
    private const bool ClimateControl = true;
    private PromotionManager _promotionManager = new();
    private DepositManager _depositManager = new();
    private Credentials _clientCredentials;
    private Credentials _adminCredentials;

    [TestInitialize]
    public void SetUp()
    {
        _promotionManager = new PromotionManager();
        _depositManager = new DepositManager();
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

        _adminCredentials = authManager.Login(new LoginDto()
            { Email = admin.Email, Password = admin.Password });
        _clientCredentials = authManager.Login(new LoginDto()
            { Email = client.Email, Password = client.Password });

        var promotion = new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _promotionManager.Add(promotion, _adminCredentials);
    }

    [TestMethod]
    public void TestCanAddDepositWithValidData()
    {
        // Arrange
        var promotionList = new List<Promotion>() { _promotionManager.Promotions[0] };
        var deposit = new Deposit(1, Area, Size, ClimateControl, promotionList);

        // Act
        _depositManager.Add(deposit, _adminCredentials);

        // Assert
        Assert.AreEqual(1, _depositManager.GetAllDeposits(_adminCredentials).Count);
    }

    [TestMethod]
    public void TestCanDeleteDeposit()
    {
        // Arrange
        var promotionList = new List<Promotion>() { _promotionManager.Promotions[0] };
        var deposit = new Deposit(1, Area, Size, ClimateControl, promotionList);
        _depositManager.Add(deposit, _adminCredentials);

        // Act
        _depositManager.Delete(1, _adminCredentials);

        // Assert
        Assert.AreEqual(0, _depositManager.GetAllDeposits(_adminCredentials).Count);
    }

    [TestMethod]
    public void TestCantDeleteNonExistentDeposit()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => _depositManager.Delete(1, _adminCredentials));

        // Assert
        Assert.AreEqual("Deposit not found", exception.Message);
    }

    [TestMethod]
    public void TestCantAddDepositIfNotAdministrator()
    {
        // Arrange
        var promotionList = new List<Promotion>() { _promotionManager.Promotions[0] };
        var deposit = new Deposit(1, Area, Size, ClimateControl, promotionList);

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
        var promotionList = new List<Promotion>() { _promotionManager.Promotions[0] };
        var deposit = new Deposit(1, Area, Size, ClimateControl, promotionList);
        _depositManager.Add(deposit, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() => _depositManager.Delete(1, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage deposits.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetAllDepositsIfNotAdministrator()
    {
        // Arrange
        var promotionList = new List<Promotion>() { _promotionManager.Promotions[0] };
        var deposit = new Deposit(1, Area, Size, ClimateControl, promotionList);
        _depositManager.Add(deposit, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() => _depositManager.GetAllDeposits(_clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage deposits.", exception.Message);
    }
}