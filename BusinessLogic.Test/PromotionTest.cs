using BusinessLogic.Domain;

namespace BusinessLogic.Test;

[TestClass]
public class PromotionTest
{
    private const string Label = "label";
    private const int Discount = 50;
    private const int MaxDiscount = 70;
    private const int MinDiscount = 5;
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);
    private readonly DateOnly _tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

    [TestMethod]
    public void TestCanCreatePromotionWithValidData()
    {
        // Act
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);

        // Assert
        Assert.IsNotNull(promotion);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithLabelWithSymbols()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => new Promotion(1, "l@bel", Discount, _today, _tomorrow));

        // Assert
        Assert.AreEqual("Label must not contain symbols.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithLabelLengthGreaterThan20()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new Promotion(1, "label with more than 20 characters", Discount, _today, _tomorrow));

        // Assert
        Assert.AreEqual("Label length must be lesser or equal than 20.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithDateFromGreaterThanDateTo()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => new Promotion(1, Label, Discount, _tomorrow, _today));

        // Assert
        Assert.AreEqual("The starting date of the promotion must not be later than the ending date.",
            exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithDateToLesserThanToday()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(1, Label, Discount,
            DateOnly.FromDateTime(DateTime.Now.AddDays(-2)), DateOnly.FromDateTime(DateTime.Now.AddDays(-1))));

        // Assert
        Assert.AreEqual("The ending date of the promotion cannot be in the past.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithDiscountsLesserThanMinDiscount()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(1, Label, MinDiscount-1, _today, _tomorrow));

        // Assert
        Assert.AreEqual("Discount must be between 5% and 70%.", exception.Message);
    }


    [TestMethod]
    public void TestCantCreatePromotionWithDiscountGreaterThanMaxDiscount()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(1, Label, MaxDiscount+1, _today, _tomorrow));

        // Assert
        Assert.AreEqual("Discount must be between 5% and 70%.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithEmptyLabel()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => new Promotion(1, "", Discount, _today, _tomorrow));

        // Assert
        Assert.AreEqual("Label must not be empty.", exception.Message);
    }
}