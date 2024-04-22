namespace BusinessLogic.Test;

[TestClass]
public class PromotionManagerTest
{
    private const string Label = "label";
    private const int Discount = 50;
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);
    private readonly DateOnly _tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

    [TestMethod]
    public void TestCanAddPromotion()
    {
        // Arrange
        var promotionManager = new PromotionManager();
        var addDto = new AddPromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
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
        var credentials = authManager.Login(adminModel.Email, adminModel.Password);

        // Act
        promotionManager.Add(addDto, credentials);

        // Assert
        Assert.AreEqual(1, promotionManager.Promotions.Count);
    }

    [TestMethod]
    public void TestCanDeletePromotion()
    {
        // Arrange
        var promotionManager = new PromotionManager();
        var addDto = new AddPromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };
        var promotion = new Promotion(1, addDto.Label, addDto.Discount, addDto.DateFrom, addDto.DateTo);
        var authManager = new AuthManager();
        var adminModel = new UserModel()
        {
            Email = "test@test.com",
            Password = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = UserRank.Administrator
        };
        authManager.Register(adminModel, adminModel.Password);
        var credentials = authManager.Login(adminModel.Email, adminModel.Password);
        promotionManager.Add(addDto, credentials);

        // Act
        promotionManager.Delete(1);

        // Assert
        Assert.IsFalse(promotionManager.Promotions.Contains(promotion));
    }

    [TestMethod]
    public void TestCanModifyPromotion()
    {
        // Arrange
        var promotionManager = new PromotionManager();
        var addDto = new AddPromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };
        var promotion = new Promotion(1, addDto.Label, addDto.Discount, addDto.DateFrom, addDto.DateTo);
        var authManager = new AuthManager();
        var adminModel = new UserModel()
        {
            Email = "test@test.com",
            Password = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = UserRank.Administrator
        };
        authManager.Register(adminModel, adminModel.Password);
        var credentials = authManager.Login(adminModel.Email, adminModel.Password);
        promotionManager.Add(addDto, credentials);
        var modifyDto = new ModifyPromotionDto()
        {
            Id = 1,
            Label = "new label",
            Discount = 20,
            DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
        };

        // Act
        promotionManager.Modify(modifyDto);

        // Assert
        Assert.IsFalse(promotionManager.Promotions.Contains(promotion));
    }

    [TestMethod]
    public void TestOnlyAdministratorsCanAddPromotion()
    {
        // Arrange
        var promotionManager = new PromotionManager();
        var authManager = new AuthManager();
        var model = new AddPromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };
        var clientModel = new UserModel()
        {
            Email = "test@test.com",
            Password = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = UserRank.Client
        };
        authManager.Register(clientModel, clientModel.Password);
        var credentials = authManager.Login(clientModel.Email, clientModel.Password);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() => promotionManager.Add(model, credentials));

        // Assert
        Assert.AreEqual("Only administrators can add promotions.", exception.Message);
    }
}