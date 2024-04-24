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
    private Credentials _credentials;

    [TestInitialize]
    public void SetUp()
    {
        _authManager = new AuthManager();
        _promotionManager = new PromotionManager();
        _depositManager = new DepositManager();
        var userModel = new RegisterDto()
        {
            NameSurname = "Name Surname",
            Email = "test@test.com",
            Password = "12345678@mE",
            PasswordConfirmation = "12345678@mE",
            Rank = "Administrator"
        };
        _authManager.Register(userModel);
        var loginModel = new LoginDto()
        {
            Email = userModel.Email,
            Password = userModel.Password
        };
        _credentials = _authManager.Login(loginModel);
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
        _promotionManager.Add(promotionModel1, _credentials);
        _promotionManager.Add(promotionModel2, _credentials);
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
        _depositManager.Add(depositAddDto, _credentials, _promotionManager);

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
        _depositManager.Add(depositAddDto, _credentials, _promotionManager);

        // Act
        _depositManager.Delete(1, _credentials);

        // Assert
        Assert.AreEqual(0, _depositManager.Deposits.Count);
    }
}