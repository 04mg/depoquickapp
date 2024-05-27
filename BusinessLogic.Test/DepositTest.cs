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
    private AuthManager _authManager = new();
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

        var loginModel = new LoginDto
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

    [TestMethod]
    public void TestCanAddAvailabilityPeriods()
    {
        // Arrange
        var deposit = new Deposit(Name, Area, Size, ClimateControl, _promotionList);
        var availabilityPeriod = new DateRange(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)));        
        // Act
        deposit.AddAvailabilityPeriod(availabilityPeriod);
        
        // Assert
        Assert.AreEqual(1, deposit.GetAvailablePeriods().Count);
    }
    
    [TestMethod]
    public void TestOverlappingAvailabilityPeriodsShouldMerge()
    {
        // Arrange
        var deposit = new Deposit(Name, Area, Size, ClimateControl, _promotionList);
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
        var availabilityPeriod1 = new DateRange(startDate, DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        var availabilityPeriod2 = new DateRange(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), endDate);

        // Act
        deposit.AddAvailabilityPeriod(availabilityPeriod1);
        deposit.AddAvailabilityPeriod(availabilityPeriod2);

        // Assert
        Assert.AreEqual(1, deposit.GetAvailablePeriods().Count);
        Assert.AreEqual(startDate, deposit.GetAvailablePeriods()[0].StartDate);
        Assert.AreEqual(endDate, deposit.GetAvailablePeriods()[0].EndDate);
    }
    
    [TestMethod]
    public void TestCanRemoveTheTotalityOfAnAvailabilityPeriod()
    {
        // Arrange
        var deposit = new Deposit(Name, Area, Size, ClimateControl, _promotionList);
        var availabilityPeriod = new DateRange(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        deposit.AddAvailabilityPeriod(availabilityPeriod);
        
        // Act
        deposit.RemoveAvailabilityPeriod(availabilityPeriod);
        
        // Assert
        Assert.AreEqual(0, deposit.GetAvailablePeriods().Count);
    }
    
    [TestMethod]
    public void TestCanRemovePartOfAnAvailabilityPeriod()
    {
        // Arrange
        var deposit = new Deposit(Name, Area, Size, ClimateControl, _promotionList);
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var middleDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
        var availabilityPeriod = new DateRange(startDate, endDate);
        deposit.AddAvailabilityPeriod(availabilityPeriod);
        var rangeToRemove = new DateRange(startDate, middleDate);
        
        // Act
        deposit.RemoveAvailabilityPeriod(rangeToRemove);
        
        // Assert
        Assert.AreEqual(1, deposit.GetAvailablePeriods().Count);
        Assert.AreEqual(middleDate.AddDays(1), deposit.GetAvailablePeriods()[0].StartDate);
        Assert.AreEqual(endDate, deposit.GetAvailablePeriods()[0].EndDate);
    }
    
    [TestMethod]
    public void TestCanRemoveTheMiddleOfAnAvailabilityPeriod()
    {
        // Arrange
        var deposit = new Deposit(Name, Area, Size, ClimateControl, _promotionList);
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var middleStartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
        var middleEndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
        var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5));
        var availabilityPeriod = new DateRange(startDate, endDate);
        deposit.AddAvailabilityPeriod(availabilityPeriod);
        var rangeToRemove = new DateRange(middleStartDate, middleEndDate);
        
        // Act
        deposit.RemoveAvailabilityPeriod(rangeToRemove);
        
        // Assert
        Assert.AreEqual(2, deposit.GetAvailablePeriods().Count);
        Assert.AreEqual(startDate, deposit.GetAvailablePeriods()[0].StartDate);
        Assert.AreEqual(middleStartDate.AddDays(-1), deposit.GetAvailablePeriods()[0].EndDate);
        Assert.AreEqual(middleEndDate.AddDays(1), deposit.GetAvailablePeriods()[1].StartDate);
        Assert.AreEqual(endDate, deposit.GetAvailablePeriods()[1].EndDate);
    }
}