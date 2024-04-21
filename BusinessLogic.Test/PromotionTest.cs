namespace BusinessLogic.Test;

[TestClass]
public class PromotionTest
{
    private const string Label = "label";
    private const int Discount = 50;
    private readonly string _dateFrom = DateTime.Now.ToString("yyyy-MM-dd");
    private readonly string _dateTo = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

    [TestMethod]
    public void TestCanCreatePromotionWithValidData()
    {
        // Arrange
        var model = new PromotionModel
        {
            Label = Label,
            Discount = Discount,
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
            Discount = Discount,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        // Assert
        Assert.AreEqual("Label must not contain symbols.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithLabelLengthGreaterThan20()
    {
        //Arrange
        var model = new PromotionModel
        {
            Label = "label with more than 20 characters",
            Discount = Discount,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        //Assert
        Assert.AreEqual("Label length must be lesser or equal than 20.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithDateFromGreaterThanDateTo()
    {
        // Arrange
        var model = new PromotionModel
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _dateTo,
            DateTo = _dateFrom
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        // Assert
        Assert.AreEqual("The starting date of the promotion must not be later than the ending date.",
            exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithDateToLesserThanToday()
    {
        // Arrange
        var model = new PromotionModel
        {
            Label = Label,
            Discount = Discount,
            DateFrom = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"),
            DateTo = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        // Assert
        Assert.AreEqual("The ending date of the promotion cannot be in the past.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithDiscountsLesserThan5()
    {
        //Arrange
        var model = new PromotionModel
        {
            Label = Label,
            Discount = 4,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        //Assert
        Assert.AreEqual("Discount must be between 5% and 70%.", exception.Message);
    }


    [TestMethod]
    public void TestCantCreatePromotionWithDiscountGreaterThan70()
    {
        //Arrange
        var model = new PromotionModel
        {
            Label = Label,
            Discount = 71,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        //Assert
        Assert.AreEqual("Discount must be between 5% and 70%.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithEmptyLabel()
    {
        // Arrange
        var model = new PromotionModel
        {
            Label = "",
            Discount = Discount,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(model));

        // Assert
        Assert.AreEqual("Label must not be empty.", exception.Message);
    }

    [TestMethod]
    public void TestCanAddPromotion()
    {
        // Arrange
        var promotionManager = new PromotionManager();
        var model = new PromotionModel
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };

        // Act
        promotionManager.Add(model);
        
        // Assert
        Assert.AreEqual(1, promotionManager.Promotions.Count);
    }
}