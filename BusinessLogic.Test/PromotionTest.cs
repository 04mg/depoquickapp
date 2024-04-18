namespace BusinessLogic.Test;

[TestClass]
public class PromotionTest
{
    [TestMethod]
    public void TestCanCreatePromotionWithValidData()
    {
        // Arrange
        DateOnly from = DateOnly.FromDateTime(DateTime.Now);
        DateOnly to = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
       
        // Act
        var promotion = new Promotion("label", 50, from, to);

        // Assert
        Assert.IsNotNull(promotion);
    }
}