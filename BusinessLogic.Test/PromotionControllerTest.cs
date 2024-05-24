using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Managers;

namespace BusinessLogic.Test;

[TestClass]
public class PromotionControllerTest
{
    private const string Label = "label";
    private const int Discount = 50;
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);
    private readonly DateOnly _tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
    private Credentials _adminCredentials;
    private Credentials _clientCredentials;
    private PromotionController _promotionController = new();

    [TestInitialize]
    public void Initialize()
    {
        _promotionController = new PromotionController();
        RegisterUsers();
    }

    private void RegisterUsers()
    {
        var authManager = new AuthController();

        const string passwordConfirmation = "12345678@mE";
        var admin = new User(
            "Name Surname",
            "admin@admin.com",
            "12345678@mE",
            "Administrator"
        );
        var client = new User(
            "Name Surname",
            "client@client.com",
            "12345678@mE"
        );
        authManager.Register(admin, passwordConfirmation);
        authManager.Register(client, passwordConfirmation);
        _adminCredentials = authManager.Login(new LoginDto { Email = admin.Email, Password = admin.Password });
        _clientCredentials = authManager.Login(new LoginDto { Email = client.Email, Password = client.Password });
    }

    [TestMethod]
    public void TestCanAddPromotion()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);

        // Act
        _promotionController.Add(promotion, _adminCredentials);

        // Assert
        Assert.AreEqual(1, _promotionController.GetAllPromotions(_adminCredentials).Count());
    }

    [TestMethod]
    public void TestCanDeletePromotion()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionController.Add(promotion, _adminCredentials);

        // Act
        _promotionController.Delete(1, _adminCredentials);

        // Assert
        Assert.IsFalse(_promotionController.GetAllPromotions(_adminCredentials).Contains(promotion));
    }

    [TestMethod]
    public void TestCanModifyPromotion()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionController.Add(promotion, _adminCredentials);
        var modifiedPromotion = new Promotion(1, "new label", 20, DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(3)));

        // Act
        _promotionController.Modify(1, modifiedPromotion, _adminCredentials);

        // Assert
        Assert.IsFalse(_promotionController.GetAllPromotions(_adminCredentials).Contains(modifiedPromotion));
    }

    [TestMethod]
    public void TestCantAddPromotionIfNotAdministrator()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionController.Add(promotion, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeletePromotionIfNotAdministrator()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionController.Add(promotion, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionController.Delete(1, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantModifyPromotionIfNotAdministrator()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionController.Add(promotion, _adminCredentials);
        var modifiedPromotion = new Promotion(1, "new label", 20, DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(3)));

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionController.Modify(1, modifiedPromotion, _clientCredentials));

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
                _promotionController.Modify(1, modifiedPromotion, _adminCredentials));

        // Assert
        Assert.AreEqual("Promotion not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeleteNonExistentPromotion()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                _promotionController.Delete(1, _adminCredentials));

        // Assert
        Assert.AreEqual("Promotion not found.", exception.Message);
    }

    [TestMethod]
    public void TestCanCheckIfPromotionExists()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionController.Add(promotion, _adminCredentials);

        // Act
        var exists = _promotionController.Exists(1);

        // Assert
        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void TestCanCheckIfPromotionDoesNotExist()
    {
        // Act
        var exists = _promotionController.Exists(1);

        // Assert
        Assert.IsFalse(exists);
    }

    [TestMethod]
    public void TestCantGetAllPromotionsIfNotAdministrator()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionController.Add(promotion, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionController.GetAllPromotions(_clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }
}