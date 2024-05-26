using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Logic;
using BusinessLogic.Repositories;

namespace BusinessLogic.Test;

[TestClass]
public class DepositLogicTest
{
    private const string Name = "Deposit";
    private const string Area = "A";
    private const string Size = "Small";
    private const bool ClimateControl = true;
    private Credentials _adminCredentials;
    private Credentials _clientCredentials;

    private DepositLogic _depositLogic =
        new(new DepositRepository(), new BookingRepository());

    private PromotionLogic _promotionLogic = new(new PromotionRepository(), new DepositRepository());

    private readonly AuthLogic _authLogic = new(new UserRepository());

    [TestInitialize]
    public void Initialize()
    {
        var depositRepository = new DepositRepository();
        var bookingRepository = new BookingRepository();
        var promotionRepository = new PromotionRepository();
        _depositLogic = new DepositLogic(depositRepository, bookingRepository);
        _promotionLogic = new PromotionLogic(promotionRepository, depositRepository);

        RegisterUsers();
        CreatePromotion();
    }

    private void RegisterUsers()
    {
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

        _authLogic.Register(admin, passwordConfirmation);
        _authLogic.Register(client, passwordConfirmation);

        _adminCredentials = _authLogic.Login(new LoginDto { Email = admin.Email, Password = admin.Password });
        _clientCredentials = _authLogic.Login(new LoginDto { Email = client.Email, Password = client.Password });
    }

    private void CreatePromotion()
    {
        var promotion = new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        _promotionLogic.Add(promotion, _adminCredentials);
    }

    [TestMethod]
    public void TestCanAddDepositWithValidData()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionLogic.GetAllPromotions(_adminCredentials).ToList()[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);

        // Act
        _depositLogic.Add(deposit, _adminCredentials);

        // Assert
        Assert.AreEqual(1, _depositLogic.GetAllDeposits().Count());
    }

    [TestMethod]
    public void TestCanDeleteDeposit()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionLogic.GetAllPromotions(_adminCredentials).ToList()[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositLogic.Add(deposit, _adminCredentials);

        // Act
        _depositLogic.Delete(Name, _adminCredentials);

        // Assert
        Assert.AreEqual(0, _depositLogic.GetAllDeposits().Count());
    }

    [TestMethod]
    public void TestCantDeleteNonExistentDeposit()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => _depositLogic.Delete(Name, _adminCredentials));

        // Assert
        Assert.AreEqual("Deposit not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantAddDepositIfNotAdministrator()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionLogic.GetAllPromotions(_adminCredentials).ToList()[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _depositLogic.Add(deposit, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage deposits.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeleteDepositIfNotAdministrator()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionLogic.GetAllPromotions(_adminCredentials).ToList()[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositLogic.Add(deposit, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _depositLogic.Delete(Name, _clientCredentials));

        // Assert
        Assert.AreEqual("Only administrators can manage deposits.", exception.Message);
    }

    [TestMethod]
    public void TestCanGetAllDeposits()
    {
        // Arrange
        var promotionList = new List<Promotion>
            { _promotionLogic.GetAllPromotions(_adminCredentials).ToList()[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositLogic.Add(deposit, _adminCredentials);

        // Act
        var deposits = _depositLogic.GetAllDeposits().ToList();

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
            { _promotionLogic.GetAllPromotions(_adminCredentials).ToList()[0] };
        var deposit = new Deposit(Name, Area, Size, ClimateControl, promotionList);
        _depositLogic.Add(deposit, _adminCredentials);

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => _depositLogic.Add(deposit, _adminCredentials));

        // Assert
        Assert.AreEqual("Deposit name is already taken.", exception.Message);
    }
}