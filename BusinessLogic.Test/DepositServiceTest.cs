using BusinessLogic.Calculators;
using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Repositories;
using BusinessLogic.Services;

namespace BusinessLogic.Test;

[TestClass]
public class DepositServiceTest
{
    private const string Name = "Deposit";
    private const string Area = "A";
    private const string Size = "Small";
    private const bool ClimateControl = true;
    private Credentials _adminCredentials;
    private Credentials _clientCredentials;

    private DepositRepository _depositRepository = new();
    private BookingRepository _bookingRepository = new();
    private PromotionRepository _promotionRepository = new();

    private DepositService _depositService =
        new(new DepositRepository(), new BookingRepository(), new PromotionRepository());

    [TestInitialize]
    public void Initialize()
    {
        InitializeDepositService();
        SetCredentials();
        CreatePromotion();
    }

    private void InitializeDepositService()
    {
        _depositRepository = new DepositRepository();
        _bookingRepository = new BookingRepository();
        _promotionRepository = new PromotionRepository();
        _depositService = new DepositService(_depositRepository, _bookingRepository, _promotionRepository);
    }

    private void SetCredentials()
    {
        _adminCredentials = new Credentials() { Email = "admin@admin.com", Rank = "Administrator" };
        _clientCredentials = new Credentials() { Email = "client@client.com", Rank = "Client" };
    }

    private void CreatePromotion()
    {
        var promotion = new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        _promotionRepository.Add(promotion);
    }

    [TestMethod]
    public void TestCanAddDepositWithValidData()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionRepository.Get(1) };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);

        // Act
        _depositService.AddDeposit(deposit, _adminCredentials);

        // Assert
        Assert.AreEqual(1, _depositService.GetAllDeposits().Count());
    }

    [TestMethod]
    public void TestCanDeleteDeposit()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionRepository.Get(1) };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositService.AddDeposit(deposit, _adminCredentials);

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
            Assert.ThrowsException<ArgumentException>(() => _depositService.DeleteDeposit(Name, _adminCredentials));

        // Assert
        Assert.AreEqual("Deposit not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantAddDepositIfNotAdministrator()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionRepository.Get(1) };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _depositService.AddDeposit(deposit, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage deposits.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeleteDepositIfNotAdministrator()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionRepository.Get(1) };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositService.AddDeposit(deposit, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _depositService.DeleteDeposit(Name, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage deposits.", exception.Message);
    }

    [TestMethod]
    public void TestCanGetAllDeposits()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionRepository.Get(1) };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositService.AddDeposit(deposit, _adminCredentials);

        // Act
        var deposits = _depositService.GetAllDeposits().ToList();

        // Assert
        Assert.IsNotNull(deposits);
        Assert.AreEqual(1, deposits.Count);
        Assert.AreEqual(Name, deposits[0].Name);
        Assert.AreEqual(Area, deposits[0].Area);
        Assert.AreEqual(Size, deposits[0].Size);
        Assert.AreEqual(ClimateControl, deposits[0].ClimateControl);
        Assert.AreEqual(promotionList, deposits[0].Promotions);
    }

    [TestMethod]
    public void TestCantAddDepositIfNameIsAlreadyTaken()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionRepository.Get(1) };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositService.AddDeposit(deposit, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => _depositService.AddDeposit(deposit, _adminCredentials));

        // Assert
        Assert.AreEqual("Deposit name is already taken.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeleteDepositIncludedInBookings()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionRepository.Get(1) };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        var dateRange = new DateRange(DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        _depositService.AddDeposit(deposit, _adminCredentials);
        _depositService.AddAvailabilityPeriod(Name, dateRange, _adminCredentials);
        var client = new User(
            "Name Surname",
            "client@client.com",
            "12345678@mE"
        );
        var booking = new Booking(1, deposit, client, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        _bookingRepository.Add(booking);


        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => _depositService.DeleteDeposit(Name, _adminCredentials));

        // Assert
        Assert.AreEqual("There are existing bookings for this deposit.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateDepositIfPromotionDoesNotExist()
    {
        // Arrange
        var deposit = new Deposit(Name, Area, Size, ClimateControl, new List<Promotion>
        {
            new Promotion(2, "label", 50, DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
        });

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            _depositService.AddDeposit(deposit, _adminCredentials);
        });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Promotion not found."));
    }
}