namespace BusinessLogic.Test;

[TestClass]
public class PromotionTest
{
    [TestMethod]
    public void TestCanCreatePromotionWithValidData()
    {
        // Arrange
        var from = DateOnly.FromDateTime(DateTime.Now);
        var to = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
       
        // Act
        var promotion = new Promotion("label", 50, from, to);

        // Assert
        Assert.IsNotNull(promotion);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithLabelWithSymbols()
    {
        // Arrange
        var from = DateOnly.FromDateTime(DateTime.Now);
        var to = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion("l@bel", 50, from, to));
        
        // Assert
        Assert.AreEqual("Label format is invalid, it can't contain symbols", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithLengthGreaterThan20()
    {
        //Arrange
        var from = DateOnly.FromDateTime(DateTime.Now);
        var to = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var invalidLabel = new string('a', 21);
        
        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion(invalidLabel, 50, from, to));
        
        //Assert
        Assert.AreEqual("Label format is invalid, length must be lesser or equal than 20", exception.Message);
    }

    [TestMethod]
    public void TestCantCreatePromotionWithDateFromGreaterThanDateTo()
    {
        // Arrange
        var from = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
        var to = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion("label", 50, from, to));
        
        // Assert
        Assert.AreEqual("DateFrom must be lesser than DateTo", exception.Message);
    }
}