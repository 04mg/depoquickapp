namespace BusinessLogic.Test;

[TestClass]
public class PromotionTest
{
    private readonly string _label = "label";
    private readonly int _discount = 50;
    private readonly string _dateFrom = DateTime.Now.ToString("yyyy-MM-dd");
    private readonly string _dateTo = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

    [TestMethod]
    public void TestCanCreatePromotionWithValidData()
    {
        // Arrange
        var model = new PromotionModel
        {
            Label = _label,
            Discount = _discount,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };

        // Act
        var promotion = new Promotion(model);

        // Assert
        Assert.IsNotNull(promotion);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithLabelWithSymbols()
    {
        // Arrange
        var model = new PromotionModel
        {
            Label = "l@bel",
            Discount = _discount,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        // Assert
        Assert.AreEqual("Label format is invalid, it can't contain symbols", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithLabelLengthGreaterThan20()
    {
        //Arrange
        var model = new PromotionModel
        {
            Label = "label with more than 20 characters",
            Discount = _discount,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        //Assert
        Assert.AreEqual("Label format is invalid, length must be lesser or equal than 20", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithDateFromGreaterThanDateTo()
    {
        // Arrange
        var model = new PromotionModel
        {
            Label = _label,
            Discount = _discount,
            DateFrom = _dateTo,
            DateTo = _dateFrom
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        // Assert
        Assert.AreEqual("DateFrom must be lesser than DateTo", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithDiscountsLesserThan5()
    {
        //Arrange
        var model = new PromotionModel
        {
            Label = _label,
            Discount = 4,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        //Assert
        Assert.AreEqual("Invalid discount, it must be between 5% and 70%", exception.Message);
    }


    [TestMethod]
    public void TestCantCreatePromotionWithDiscountGreaterThan70()
    {
        //Arrange
        var model = new PromotionModel
        {
            Label = _label,
            Discount = 71,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        //Assert
        Assert.AreEqual("Invalid discount, it must be between 5% and 70%", exception.Message);
    }
}