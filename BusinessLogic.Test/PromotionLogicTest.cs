using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Enums;
using BusinessLogic.Logic;
using BusinessLogic.Repositories;

namespace BusinessLogic.Test;

[TestClass]
public class PromotionLogicTest
{
    private const string Label = "label";
    private const int Discount = 50;
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);
    private readonly DateOnly _tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
    private Credentials _adminCredentials;
    private Credentials _clientCredentials;
    private PromotionLogic _promotionLogic = new(new PromotionRepository(), new DepositRepository());

    private DepositLogic _depositLogic =
        new(new DepositRepository(), new BookingRepository(), new PromotionRepository());

    [TestInitialize]
    public void Initialize()
    {
        var promotionRepository = new PromotionRepository();
        var depositRepository = new DepositRepository();
        _depositLogic = new DepositLogic(depositRepository, new BookingRepository(), promotionRepository);
        _promotionLogic = new PromotionLogic(promotionRepository, depositRepository);
        RegisterUsers();
    }

    private void RegisterUsers()
    {
        var authManager = new AuthLogic(new UserRepository());

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
        _adminCredentials = authManager.Login(admin.Email, admin.Password);
        _clientCredentials = authManager.Login(client.Email, client.Password);
    }

    [TestMethod]
    public void TestCanAddPromotion()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);

        // Act
        _promotionLogic.AddPromotion(promotion, _adminCredentials);

        // Assert
        Assert.AreEqual(1, _promotionLogic.GetAllPromotions(_adminCredentials).Count());
    }

    [TestMethod]
    public void TestCanDeletePromotion()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionLogic.AddPromotion(promotion, _adminCredentials);

        // Act
        _promotionLogic.DeletePromotion(1, _adminCredentials);

        // Assert
        Assert.IsFalse(_promotionLogic.GetAllPromotions(_adminCredentials).Contains(promotion));
    }

    [TestMethod]
    public void TestCanModifyPromotion()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionLogic.AddPromotion(promotion, _adminCredentials);
        var modifiedPromotion = new Promotion(1, "new label", 20, DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(3)));

        // Act
        _promotionLogic.ModifyPromotion(1, modifiedPromotion, _adminCredentials);

        // Assert
        Assert.IsFalse(_promotionLogic.GetAllPromotions(_adminCredentials).Contains(modifiedPromotion));
    }

    [TestMethod]
    public void TestCantAddPromotionIfNotAdministrator()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionLogic.AddPromotion(promotion, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeletePromotionIfNotAdministrator()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionLogic.AddPromotion(promotion, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionLogic.DeletePromotion(1, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantModifyPromotionIfNotAdministrator()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionLogic.AddPromotion(promotion, _adminCredentials);
        var modifiedPromotion = new Promotion(1, "new label", 20, DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(3)));

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionLogic.ModifyPromotion(1, modifiedPromotion, _clientCredentials));

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
                _promotionLogic.ModifyPromotion(1, modifiedPromotion, _adminCredentials));

        // Assert
        Assert.AreEqual("Promotion not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeleteNonExistentPromotion()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                _promotionLogic.DeletePromotion(1, _adminCredentials));

        // Assert
        Assert.AreEqual("Promotion not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetAllPromotionsIfNotAdministrator()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionLogic.AddPromotion(promotion, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionLogic.GetAllPromotions(_clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeletePromotionIncludedInDeposits()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        _promotionLogic.AddPromotion(promotion, _adminCredentials);
        var deposit = new Deposit("Deposit", "A", "Large", true, new List<Promotion>() { promotion });
        _depositLogic.AddDeposit(deposit, _adminCredentials);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => _promotionLogic.DeletePromotion(1, _adminCredentials));

        // Assert
        Assert.AreEqual("There are existing deposits for this promotion.", exception.Message);
    }
}