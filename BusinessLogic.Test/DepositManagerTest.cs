namespace BusinessLogic.Test;

[TestClass]
public class DepositManagerTest
{
    private const string Area = "A";
    private const string Size = "Small";
    private const bool ClimateControl = true;
    private AuthManager _authManager = new();
    private PromotionManager _promotionManager = new();
    private DepositManager _depositManager = new();
    private List<int> _promotionList = new();
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

        var promotionModel1 = new AddPromotionDto()
        {
            Label = "label",
            Discount = 50,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        var promotionModel2 = new AddPromotionDto()
        {
            Label = "label",
            Discount = 50,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        _promotionManager.Add(promotionModel1, _adminCredentials);
        _promotionManager.Add(promotionModel2, _adminCredentials);
        _promotionList = new List<int>() { 1, 2 };
    }

    [TestMethod]
    public void TestCanAddDepositWithValidData()
    {
        // Arrange
        var depositAddDto = new AddDepositDto()
        {
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            PromotionList = _promotionList
        };
        // Act
        _depositManager.Add(depositAddDto, _adminCredentials, _promotionManager);

        // Assert
        Assert.AreEqual(1, _depositManager.Deposits.Count);
    }

    [TestMethod]
    public void TestCanDeleteDeposit()
    {
        // Arrange
        var depositAddDto = new AddDepositDto()
        {
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            PromotionList = _promotionList
        };
        _depositManager.Add(depositAddDto, _adminCredentials, _promotionManager);

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
        var depositAddDto = new AddDepositDto()
        {
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            PromotionList = _promotionList
        };

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() => _depositManager.Add(depositAddDto, _clientCredentials, _promotionManager));

        // Assert
        Assert.AreEqual("Only administrators can add deposits.", exception.Message);
    }
}