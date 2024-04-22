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

        var adminModel = new RegisterDto()
        {
            Email = "admin@admin.com",
            Password = "12345678@mE",
            PasswordConfirmation = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = "Administrator"
        };

        var clientModel = new RegisterDto()
        {
            Email = "client@client.com",
            Password = "12345678@mE",
            PasswordConfirmation = "12345678@mE",
            NameSurname = "Name Surname",
            Rank = "Client"
        };

        authManager.Register(adminModel);
        authManager.Register(clientModel);
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
        _promotionManager.Delete(1, _adminCredentials);

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
        _promotionManager.Modify(modifyDto, _adminCredentials);

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
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
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
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantModifyPromotionIfNotAdministrator()
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

        var modifyDto = new ModifyPromotionDto()
        {
            Id = 1,
            Label = "new label",
            Discount = 20,
            DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
        };

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionManager.Modify(modifyDto, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantModifyNonExistentPromotion()
    {
        // Arrange
        var modifyDto = new ModifyPromotionDto()
        {
            Id = 1,
            Label = "new label",
            Discount = 20,
            DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
        };

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                _promotionManager.Modify(modifyDto, _adminCredentials));

        // Assert
        Assert.AreEqual("Promotion not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeleteNonExistentPromotion()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                _promotionManager.Delete(1, _adminCredentials));

        // Assert
        Assert.AreEqual("Promotion not found.", exception.Message);
    }
}