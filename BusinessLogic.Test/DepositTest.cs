using BusinessLogic.Domain;

namespace BusinessLogic.Test;

[TestClass]
public class DepositTest
{
    private const string Name = "Deposit";
    private const string Area = "A";
    private const string Size = "Small";
    private const bool ClimateControl = true;

    private readonly List<Promotion> _promotionList =
        new List<Promotion>
        {
            new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
            new Promotion(2, "label", 50, DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
        };

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