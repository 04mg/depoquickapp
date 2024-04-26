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
        _adminCredentials = authManager.Login(new LoginDto()
            { Email = adminModel.Email, Password = adminModel.Password });
        _clientCredentials = authManager.Login(new LoginDto()
            { Email = clientModel.Email, Password = clientModel.Password });
    }

    [TestMethod]
    public void TestCanAddPromotion()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);

        // Act
        _promotionManager.Add(promotion, _adminCredentials);

        // Assert
        Assert.AreEqual(1, _promotionManager.Promotions.Count);
    }

    [TestMethod]
    public void TestCanDeletePromotion()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionManager.Add(promotion, _adminCredentials);

        // Act
        _promotionManager.Delete(1, _adminCredentials);

        // Assert
        Assert.IsFalse(_promotionManager.Promotions.Contains(promotion));
    }

    [TestMethod]
    public void TestCanModifyPromotion()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionManager.Add(promotion, _adminCredentials);
        var modifiedPromotion = new Promotion(1, "new label", 20, DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(3)));

        // Act
        _promotionManager.Modify(1, modifiedPromotion, _adminCredentials);

        // Assert
        Assert.IsFalse(_promotionManager.Promotions.Contains(modifiedPromotion));
    }

    [TestMethod]
    public void TestCantAddPromotionIfNotAdministrator()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionManager.Add(promotion, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeletePromotionIfNotAdministrator()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionManager.Add(promotion, _adminCredentials);

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
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionManager.Add(promotion, _adminCredentials);
        var modifiedPromotion = new Promotion(1, "new label", 20, DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(3)));

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionManager.Modify(1, modifiedPromotion, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantModifyNonExistentPromotion()
    {
        // Arrange
        var modifiedPromotion = new Promotion(1, "new label", 20, _today, _tomorrow);

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                _promotionManager.Modify(1, modifiedPromotion, _adminCredentials));

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

    [TestMethod]
    public void TestCanCheckIfPromotionExists()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionManager.Add(promotion, _adminCredentials);

        // Act
        var exists = _promotionManager.Exists(1);

        // Assert
        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void TestCanCheckIfPromotionDoesNotExist()
    {
        // Act
        var exists = _promotionManager.Exists(1);

        // Assert
        Assert.IsFalse(exists);
    }
}