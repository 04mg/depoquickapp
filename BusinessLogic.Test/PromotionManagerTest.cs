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
        var authManager = new AuthManager();
        var adminModel = new UserModel()
        {
            Email = "test@test.com",
            Password = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = UserRank.Administrator
        };
        authManager.Register(adminModel, adminModel.Password);
        var creds = authManager.Login(adminModel.Email, adminModel.Password);

        // Act
        promotionManager.Add(model, creds);

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
        var authManager = new AuthManager();
        var adminModel = new UserModel()
        {
            Email = "test@test.com",
            Password = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = UserRank.Administrator
        };
        authManager.Register(adminModel, adminModel.Password);
        var creds = authManager.Login(adminModel.Email, adminModel.Password);
        promotionManager.Add(model,creds);
        
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
        var authManager = new AuthManager();
        var adminModel = new UserModel()
        {
            Email = "test@test.com",
            Password = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = UserRank.Administrator
        };
        authManager.Register(adminModel, adminModel.Password);
        var creds = authManager.Login(adminModel.Email, adminModel.Password);
        promotionManager.Add(model,creds);
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

    [TestMethod]
    public void TestOnlyAdministratorsCanAddPromotion()
    {
        // Arrange
        var promotionManager = new PromotionManager();
        var authManager = new AuthManager();
        var model = new PromotionModel
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _dateFrom,
            DateTo = _dateTo
        };
        var clientModel = new UserModel()
        {
            Email = "test@test.com",
            Password = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = UserRank.Client
        };
        authManager.Register(clientModel, clientModel.Password);
        var creds = authManager.Login(clientModel.Email, clientModel.Password);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() => promotionManager.Add(model, creds));

        // Assert
        Assert.AreEqual("Only administrators can add promotions.", exception.Message);
    }
}