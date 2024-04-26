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

        var adminModel = new RegisterDto()
        {
            Email = "admin@admin.com",
            Password = "12345678@mE",
            PasswordConfirmation = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = "Administrator"
        };
        var clientModel = new RegisterDto()
        {
            Email = "client@client.com",
            Password = "12345678@mE",
            PasswordConfirmation = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = "Client"
        };

        authManager.Register(adminModel);
        authManager.Register(clientModel);

        _adminCredentials = authManager.Login(new LoginDto()
            { Email = adminModel.Email, Password = adminModel.Password });
        _clientCredentials = authManager.Login(new LoginDto()
            { Email = clientModel.Email, Password = clientModel.Password });

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
        Assert.AreEqual(1, _depositManager.Deposits.Count);
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
        Assert.AreEqual(0, _depositManager.Deposits.Count);
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
}