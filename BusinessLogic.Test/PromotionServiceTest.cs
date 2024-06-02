using BusinessLogic.DTOs;
using BusinessLogic.Services;
using DataAccess;
using DataAccess.Repositories;
using Domain;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Test;

[TestClass]
public class PromotionServiceTest
{
    private const string Label = "label";
    private const int Discount = 50;
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);
    private readonly DateOnly _tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
    private Credentials _adminCredentials;
    private Credentials _clientCredentials;
    private PromotionRepository _promotionRepository = null!;
    private DepositRepository _depositRepository = null!;
    private PromotionService _promotionService = null!;

    [TestInitialize]
    public void Initialize()
    {
        InitializePromotionService();
        SetCredentials();
    }

    private void InitializePromotionService()
    {
        var testsContext = new ProgramTest();
        using var scope = testsContext.ServiceProvider.CreateScope();
        _promotionRepository = scope.ServiceProvider.GetRequiredService<PromotionRepository>();
        _depositRepository = scope.ServiceProvider.GetRequiredService<DepositRepository>();
        _promotionService = new PromotionService(_promotionRepository, _depositRepository);
    }

    private void SetCredentials()
    {
        _adminCredentials = new Credentials() { Email = "admin@admin.com", Rank = "Administrator" };
        _clientCredentials = new Credentials() { Email = "client@client.com", Rank = "Client" };
    }

    [TestMethod]
    public void TestCanAddPromotion()
    {
        // Arrange
        var promotionDto = new PromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };

        // Act
        _promotionService.AddPromotion(promotionDto, _adminCredentials);

        // Assert
        Assert.AreEqual(1, _promotionService.GetAllPromotions(_adminCredentials).Count());
    }

    [TestMethod]
    public void TestCanDeletePromotion()
    {
        // Arrange
        var promotionDto = new PromotionDto
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };
        _promotionService.AddPromotion(promotionDto, _adminCredentials);

        // Act
        _promotionService.DeletePromotion(1, _adminCredentials);

        // Assert
        Assert.IsFalse(_promotionService.GetAllPromotions(_adminCredentials).Contains(promotionDto));
    }

    [TestMethod]
    public void TestCanModifyPromotion()
    {
        // Arrange
        var promotionDto = new PromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };
        _promotionService.AddPromotion(promotionDto, _adminCredentials);
        var modifiedPromotionDto = new PromotionDto()
        {
            Label = "new label",
            Discount = 20,
            DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
        };

        // Act
        _promotionService.ModifyPromotion(1, modifiedPromotionDto, _adminCredentials);

        // Assert
        Assert.IsFalse(_promotionService.GetAllPromotions(_adminCredentials).Contains(modifiedPromotionDto));
    }

    [TestMethod]
    public void TestCantAddPromotionIfNotAdministrator()
    {
        // Arrange
        var promotionDto = new PromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionService.AddPromotion(promotionDto, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeletePromotionIfNotAdministrator()
    {
        // Arrange
        var promotionDto = new PromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };
        _promotionService.AddPromotion(promotionDto, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionService.DeletePromotion(1, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantModifyPromotionIfNotAdministrator()
    {
        // Arrange
        var promotionDto = new PromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };
        _promotionService.AddPromotion(promotionDto, _adminCredentials);
        var modifiedPromotionDto = new PromotionDto()
        {
            Label = "new label",
            Discount = 20,
            DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
        };

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionService.ModifyPromotion(1, modifiedPromotionDto, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantModifyNonExistentPromotion()
    {
        // Arrange
        var modifiedPromotionDto = new PromotionDto()
        {
            Label = "new label",
            Discount = 20,
            DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
        };

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                _promotionService.ModifyPromotion(1, modifiedPromotionDto, _adminCredentials));

        // Assert
        Assert.AreEqual("Promotion not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeleteNonExistentPromotion()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                _promotionService.DeletePromotion(1, _adminCredentials));

        // Assert
        Assert.AreEqual("Promotion not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetAllPromotionsIfNotAdministrator()
    {
        // Arrange
        var promotionDto = new PromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };
        _promotionService.AddPromotion(promotionDto, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _promotionService.GetAllPromotions(_clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage promotions.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeletePromotionIncludedInDeposits()
    {
        // Arrange
        var promotion = new Promotion(1, Label, Discount, _today, _tomorrow);
        var promotionDto = new PromotionDto()
        {
            Label = Label,
            Discount = Discount,
            DateFrom = _today,
            DateTo = _tomorrow
        };
        _promotionService.AddPromotion(promotionDto, _adminCredentials);
        var deposit = new Deposit("Deposit", "A", "Large", true, new List<Promotion>() { promotion });
        _depositRepository.Add(deposit);

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => _promotionService.DeletePromotion(1, _adminCredentials));

        // Assert
        Assert.AreEqual("There are existing deposits for this promotion.", exception.Message);
    }
}