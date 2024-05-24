using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Managers;

namespace BusinessLogic.Test;

[TestClass]
public class DepositTest
{
    private const string Name = "Deposit";
    private const string Area = "A";
    private const string Size = "Small";
    private const bool ClimateControl = true;
    private readonly PromotionManager _promotionManager = new();
    private AuthController _authController = new();
    private List<Promotion> _promotionList = new();

    [TestInitialize]
    public void Initialize()
    {
        _authController = new AuthController();
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

        _authController.Register(admin, passwordConfirmation);

        var loginModel = new LoginDto
        {
            Email = admin.Email,
            Password = admin.Password
        };

        var credentials = _authController.Login(loginModel);

        var promotion1 = new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        var promotion2 = new Promotion(2, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        _promotionManager.Add(promotion1, credentials);
        _promotionManager.Add(promotion2, credentials);

        _promotionList = new List<Promotion>
            { _promotionManager.GetAllPromotions(credentials)[0], _promotionManager.GetAllPromotions(credentials)[1] };
    }

    [TestMethod]
    public void TestCanCreateDepositWithValidData()
    {
        // Act
        var deposit = new Deposit(Name, Area, Size, ClimateControl, _promotionList);

        // Assert
        Assert.IsNotNull(deposit);
    }

    [TestMethod]
    public void TestCantCreateDepositWithInvalidArea()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new Deposit(Name, "Z", Size, ClimateControl, _promotionList));

        // Assert
        Assert.AreEqual("Area is invalid.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateDepositWithInvalidSize()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new Deposit(Name, Area, "Extra Large", ClimateControl, _promotionList));

        // Assert
        Assert.AreEqual("Size is invalid.", exception.Message);
    }
    
    [TestMethod]
    public void TestCantCreateDepositIfNameDoesNotHaveOnlyLettersAndSpaces()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new Deposit("Name123", Area, Size, ClimateControl, _promotionList));

        // Assert
        Assert.AreEqual("Name is invalid, it should only contain letters and whitespaces.", exception.Message);
    }
    
    [TestMethod]
    public void TestCantCreateDepositIfNameIsLongerThan100()
    {
        var invalidName = string.Concat(Enumerable.Repeat("a", 101));
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new Deposit(invalidName, Area, Size, ClimateControl, _promotionList));

        // Assert
        Assert.AreEqual("Name is invalid, it should be lesser or equal to 100 characters.", exception.Message);
    }
}