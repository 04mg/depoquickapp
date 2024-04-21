namespace BusinessLogic.Test;

[TestClass]
public class PromotionManagerTest
{
    private const string Label = "label";
    private const int Discount = 50;
    private readonly string _dateFrom = DateTime.Now.ToString("yyyy-MM-dd");
    private readonly string _dateTo = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
    
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
    
    [TestMethod]
    public void TestCanDeletePromotion()
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
        var promotion = new Promotion(model);
        promotionManager.Add(model);
        
        // Act
        promotionManager.Delete(model);
        
        // Assert
        Assert.IsFalse(promotionManager.Promotions.Contains(promotion));
        
    }
    
    [TestMethod]
    public void TestCanModifyPromotion()
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
        var promotion = new Promotion(model);
        promotionManager.Add(model);
        var newModel = new PromotionModel
        {
            Label = "new label",
            Discount = 20,
            DateFrom = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"),
            DateTo = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd")
        };

        // Act
        promotionManager.Modify(model, newModel);

        // Assert
        Assert.IsFalse(promotionManager.Promotions.Contains(promotion));

    }
}