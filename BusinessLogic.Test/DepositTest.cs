using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Managers;

namespace BusinessLogic.Test;

[TestClass]
public class DepositTest
{
    private const string Area = "A";
    private const string Size = "Small";
    private const bool ClimateControl = true;
    private AuthManager _authManager = new();
    private readonly PromotionManager _promotionManager = new();
    private List<Promotion> _promotionList = new();

    [TestInitialize]
    public void Initialize()
    {
        _authManager = new AuthManager();
        CreatePromotions();
    }

    private void CreatePromotions()
    {
        const string passwordConfirmation = "12345678@mE";

        var admin = new User(
            "Name Surname",
            "test@test.com",
            "12345678@mE",
            "Administrator"
        );

        _authManager.Register(admin, passwordConfirmation);

        var loginModel = new LoginDto()
        {
            Email = admin.Email,
            Password = admin.Password
        };

        var credentials = _authManager.Login(loginModel);

        var promotion1 = new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        var promotion2 = new Promotion(2, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        _promotionManager.Add(promotion1, credentials);
        _promotionManager.Add(promotion2, credentials);

        _promotionList = new List<Promotion>()
            { _promotionManager.GetAllPromotions(credentials)[0], _promotionManager.GetAllPromotions(credentials)[1] };
    }

    [TestMethod]
    public void TestCanCreateDepositWithValidData()
    {
        // Act
        var deposit = new Deposit(1, Area, Size, ClimateControl, _promotionList);

        // Assert
        Assert.IsNotNull(deposit);
    }

    [TestMethod]
    public void TestCantCreateDepositWithInvalidArea()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new Deposit(1, "Z", Size, ClimateControl, _promotionList));

        // Assert
        Assert.AreEqual("Area is invalid.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateDepositWithInvalidSize()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new Deposit(1, Area, "Extra Large", ClimateControl, _promotionList));

        // Assert
        Assert.AreEqual("Size is invalid.", exception.Message);
    }
}