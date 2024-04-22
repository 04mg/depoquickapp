namespace BusinessLogic.Test;

[TestClass]
public class PromotionManagerTest
{
    private const string Label = "label";
    private const int Discount = 50;
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);
    private readonly DateOnly _tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
    private PromotionManager _promotionManager = new();
    private Credentials _adminCredentials;
    private Credentials _clientCredentials;

    [TestInitialize]
    public void Initialize()
    {
        _promotionManager = new PromotionManager();
        var authManager = new AuthManager();

        var adminModel = new UserModel()
        {
            Email = "admin@admin.com",
            Password = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = UserRank.Administrator
        };

        var clientModel = new UserModel()
        {
            Email = "client@client.com",
            Password = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = UserRank.Client
        };

        authManager.Register(adminModel, adminModel.Password);
        authManager.Register(clientModel, clientModel.Password);
        _adminCredentials = authManager.Login(adminModel.Email, adminModel.Password);
        _clientCredentials = authManager.Login(clientModel.Email, clientModel.Password);
    }

    [TestMethod]
    public void TestCanAddPromotion()
    {
        // Arrange
        var addDto = new AddPromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };

        // Act
        _promotionManager.Add(addDto, _adminCredentials);

        // Assert
        Assert.AreEqual(1, _promotionManager.Promotions.Count);
    }

    [TestMethod]
    public void TestCanDeletePromotion()
    {
        // Arrange
        var addDto = new AddPromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };
        var promotion = new Promotion(1, addDto.Label, addDto.Discount, addDto.DateFrom, addDto.DateTo);
        _promotionManager.Add(addDto, _adminCredentials);

        // Act
        _promotionManager.Delete(1);

        // Assert
        Assert.IsFalse(_promotionManager.Promotions.Contains(promotion));
    }

    [TestMethod]
    public void TestCanModifyPromotion()
    {
        // Arrange
        var addDto = new AddPromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };

        var promotion = new Promotion(1, addDto.Label, addDto.Discount, addDto.DateFrom, addDto.DateTo);
        _promotionManager.Add(addDto, _adminCredentials);

        var modifyDto = new ModifyPromotionDto()
        {
            Id = 1,
            Label = "new label",
            Discount = 20,
            DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
        };

        // Act
        _promotionManager.Modify(modifyDto);

        // Assert
        Assert.IsFalse(_promotionManager.Promotions.Contains(promotion));
    }

    [TestMethod]
    public void TestCantAddPromotionIfNotAdministrator()
    {
        // Arrange
        var addDto = new AddPromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionManager.Add(addDto, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can add promotions.", exception.Message);
    }
    
    [TestMethod]
    public void TestCantDeletePromotionIfNotAdministrator()
    {
        // Arrange
        var addDto = new AddPromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };
        _promotionManager.Add(addDto, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionManager.Delete(1, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can delete promotions.", exception.Message);
    }
}