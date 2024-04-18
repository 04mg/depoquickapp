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
        var exception = Assert.ThrowsException<ArgumentException>(() => new Promotion("label1", 50, from, to));
        
        // Assert
        Assert.AreEqual("Label format is invalid, it can't contain symbols", exception.Message);
    }
}