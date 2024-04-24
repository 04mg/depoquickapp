namespace BusinessLogic.Test;

[TestClass]
public class DepositTest
{
    private const string Area = "A";
    private const string Size = "Small";
    private const bool ClimateControl = true;
    private AuthManager _authManager;
    private PromotionManager _promotionManager;
    private List<int> _promotionList;

    [TestInitialize]
    public void SetUp()
    {
        _authManager = new AuthManager();
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
        var credentials = _authManager.Login(loginModel);
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
        _promotionManager = new PromotionManager();
        _promotionManager.Add(promotionModel1, credentials);
        _promotionManager.Add(promotionModel2, credentials);
        _promotionList = new List<int>() { 1, 2 };
    }

    [TestMethod]
    public void TestCanCreateDepositWithValidData()
    {
        //Act
        var deposit = new Deposit(1, Area, Size, ClimateControl, _promotionList, _promotionManager);

        //Assert
        Assert.IsNotNull(deposit);
    }

    [TestMethod]
    public void TestCantCreateDepositWithInvalidArea()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new Deposit(1, "Z", Size, ClimateControl, _promotionList, _promotionManager));

        // Assert
        Assert.AreEqual("Area is invalid.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateDepositWithInvalidSize()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new Deposit(1, Area, "Extra Large", ClimateControl, _promotionList, _promotionManager));

        // Assert
        Assert.AreEqual("Size is invalid.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateDepositWithNonExistentPromotion()
    {
        // Arrange
        var wrongPromotionList = new List<int>() { 1, 3 };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new Deposit(1, Area, Size, ClimateControl, wrongPromotionList, _promotionManager));

        // Assert
        Assert.AreEqual("Promotion with id 3 does not exist.", exception.Message);
    }
}