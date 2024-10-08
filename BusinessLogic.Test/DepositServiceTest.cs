using BusinessLogic.DTOs;
using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using DataAccess;
using DataAccess.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Test;

[TestClass]
public class DepositServiceTest
{
    private const string Name = "Deposit";
    private const string Area = "A";
    private const string Size = "Small";
    private const bool ClimateControl = true;
    private Credentials _adminCredentials;
    private BookingRepository _bookingRepository = null!;
    private Credentials _clientCredentials;
    private DepositRepository _depositRepository = null!;
    private DepositService _depositService = null!;
    private PromotionDto _promotionDto;
    private PromotionRepository _promotionRepository = null!;
    private UserRepository _userRepository = null!;

    [TestInitialize]
    public void Initialize()
    {
        InitializeDepositService();
        SetCredentials();
        CreatePromotion();
    }

    private void InitializeDepositService()
    {
        var testsContext = new ProgramTest();
        using var scope = testsContext.ServiceProvider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IDbContextFactory<Context>>();
        _depositRepository = scope.ServiceProvider.GetRequiredService<DepositRepository>();
        _bookingRepository = scope.ServiceProvider.GetRequiredService<BookingRepository>();
        _promotionRepository = scope.ServiceProvider.GetRequiredService<PromotionRepository>();
        _userRepository = scope.ServiceProvider.GetRequiredService<UserRepository>();
        _depositService = new DepositService(_depositRepository, _bookingRepository, _promotionRepository);
    }

    private void SetCredentials()
    {
        _adminCredentials = new Credentials { Email = "admin@admin.com", Rank = "Administrator" };
        _clientCredentials = new Credentials { Email = "client@client.com", Rank = "Client" };
    }

    private void CreatePromotion()
    {
        var promotion = new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _promotionDto = new PromotionDto
        {
            Id = 1,
            Label = promotion.Label,
            Discount = promotion.Discount,
            DateFrom = promotion.Validity.StartDate,
            DateTo = promotion.Validity.EndDate
        };

        _promotionRepository.Add(promotion);
    }

    [TestMethod]
    public void TestCanAddDepositWithValidData()
    {
        // Arrange
        var depositDto = new DepositDto
        {
            Name = Name,
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            Promotions = new List<PromotionDto> { _promotionDto }
        };

        // Act
        _depositService.AddDeposit(depositDto, _adminCredentials);

        // Assert
        Assert.AreEqual(Name, _depositService.GetDeposit(Name).Name);
    }

    [TestMethod]
    public void TestCanDeleteDeposit()
    {
        // Arrange
        var depositDto = new DepositDto
        {
            Name = Name,
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            Promotions = new List<PromotionDto> { _promotionDto }
        };
        _depositService.AddDeposit(depositDto, _adminCredentials);

        // Act
        _depositService.DeleteDeposit(Name, _adminCredentials);

        // Assert
        Assert.AreEqual(0, _depositService.GetAllDeposits().Count());
    }

    [TestMethod]
    public void TestCantDeleteNonExistentDeposit()
    {
        // Act
        var exception =
            Assert.ThrowsException<BusinessLogicException>(() =>
                _depositService.DeleteDeposit(Name, _adminCredentials));

        // Assert
        Assert.AreEqual("Deposit not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantAddDepositIfNotAdministrator()
    {
        // Arrange
        var depositDto = new DepositDto
        {
            Name = Name,
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            Promotions = new List<PromotionDto> { _promotionDto }
        };

        // Act
        var exception =
            Assert.ThrowsException<BusinessLogicException>(() =>
                _depositService.AddDeposit(depositDto, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage deposits.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeleteDepositIfNotAdministrator()
    {
        // Arrange
        var depositDto = new DepositDto
        {
            Name = Name,
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            Promotions = new List<PromotionDto> { _promotionDto }
        };
        _depositService.AddDeposit(depositDto, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<BusinessLogicException>(() =>
                _depositService.DeleteDeposit(Name, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage deposits.", exception.Message);
    }

    [TestMethod]
    public void TestCanGetAllDeposits()
    {
        // Arrange
        var depositDto = new DepositDto
        {
            Name = Name,
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            Promotions = new List<PromotionDto> { _promotionDto }
        };
        _depositService.AddDeposit(depositDto, _adminCredentials);

        // Act
        var deposits = _depositService.GetAllDeposits().ToList();

        // Assert
        Assert.AreEqual(1, deposits.Count);
    }

    [TestMethod]
    public void TestCantAddDepositIfNameIsAlreadyTaken()
    {
        // Arrange
        var depositDto = new DepositDto
        {
            Name = Name,
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            Promotions = new List<PromotionDto> { _promotionDto }
        };
        _depositService.AddDeposit(depositDto, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<BusinessLogicException>(() =>
                _depositService.AddDeposit(depositDto, _adminCredentials));

        // Assert
        Assert.AreEqual("Deposit name is already taken.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeleteDepositIncludedInBookings()
    {
        // Arrange
        var depositDto = new DepositDto
        {
            Name = Name,
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            Promotions = new List<PromotionDto> { _promotionDto }
        };
        var dateRangeDto = new DateRangeDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        _depositService.AddDeposit(depositDto, _adminCredentials);
        _depositService.AddAvailabilityPeriod(Name, dateRangeDto, _adminCredentials);
        var client = new User(
            "Name Surname",
            "client@client.com",
            "12345678@mE"
        );
        _userRepository.Add(client);
        var deposit = _depositRepository.Get(Name);
        var booking = new Booking(1, deposit, client, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new Payment(50));
        _bookingRepository.Add(booking);

        // Act
        var exception =
            Assert.ThrowsException<BusinessLogicException>(() =>
                _depositService.DeleteDeposit(Name, _adminCredentials));

        // Assert
        Assert.AreEqual("There are existing bookings for this deposit.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateDepositIfPromotionDoesNotExist()
    {
        // Arrange
        var promotionDto = new PromotionDto
        {
            Label = "Non Existent",
            Discount = 50,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        var depositDto = new DepositDto
        {
            Name = Name,
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            Promotions = new List<PromotionDto> { promotionDto }
        };

        // Act
        var exception = Assert.ThrowsException<BusinessLogicException>(() =>
        {
            _depositService.AddDeposit(depositDto, _adminCredentials);
        });

        // Assert
        Assert.AreEqual("Promotion not found.", exception.Message);
    }

    [TestMethod]
    public void TestCanGetDepositsByAvailabilityPeriod()
    {
        // Arrange
        var depositDto = new DepositDto
        {
            Name = Name,
            Area = Area,
            Size = Size,
            ClimateControl = ClimateControl,
            Promotions = new List<PromotionDto> { _promotionDto }
        };
        var dateRangeDto = new DateRangeDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        _depositService.AddDeposit(depositDto, _adminCredentials);
        _depositService.AddAvailabilityPeriod(Name, dateRangeDto, _adminCredentials);

        // Act
        var deposits = _depositService.GetDepositsByAvailabilityPeriod(dateRangeDto);
        var availabilityPeriods = deposits.First().AvailabilityPeriods.First();

        // Assert
        Assert.AreEqual(dateRangeDto.StartDate, availabilityPeriods.StartDate);
        Assert.AreEqual(dateRangeDto.EndDate, availabilityPeriods.EndDate);
        Assert.AreEqual(1, deposits.Count());
    }

    [TestMethod]
    public void TestCantGetDepositsByAvailabilityPeriodIfDateRangeIsInThePast()
    {
        // Arrange
        var dateRangeDto = new DateRangeDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.Now)
        };

        // Act
        var exception = Assert.ThrowsException<BusinessLogicException>(() =>
        {
            _depositService.GetDepositsByAvailabilityPeriod(dateRangeDto);
        });

        // Assert
        Assert.AreEqual("Date range cannot be in the past.", exception.Message);
    }

    [TestMethod]
    public void TestCantAddDepositWithInvalidSize()
    {
        // Arrange
        var depositDto = new DepositDto
        {
            Name = Name,
            Area = Area,
            Size = "Invalid",
            ClimateControl = ClimateControl,
            Promotions = new List<PromotionDto> { _promotionDto }
        };

        // Act
        var exception = Assert.ThrowsException<BusinessLogicException>(() =>
        {
            _depositService.AddDeposit(depositDto, _adminCredentials);
        });

        // Assert
        Assert.AreEqual("Invalid size.", exception.Message);
    }

    [TestMethod]
    public void TestCantAddDepositWithInvalidArea()
    {
        // Arrange
        var depositDto = new DepositDto
        {
            Name = Name,
            Area = "Invalid",
            Size = Size,
            ClimateControl = ClimateControl,
            Promotions = new List<PromotionDto> { _promotionDto }
        };

        // Act
        var exception = Assert.ThrowsException<BusinessLogicException>(() =>
        {
            _depositService.AddDeposit(depositDto, _adminCredentials);
        });

        // Assert
        Assert.AreEqual("Invalid area.", exception.Message);
    }
}