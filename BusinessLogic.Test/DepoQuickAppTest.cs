using BusinessLogic.DTOs;
using BusinessLogic.Repositories;

namespace BusinessLogic.Test;

[TestClass]
public class DepoQuickAppTest
{
    private BookingDto _bookingDto = new();
    private DepositDto _depositDto;
    private DateRangeDto _dateRangeDto;
    private PromotionDto _addPromotionDto;

    private DepoQuickApp _app = new(new UserRepository(), new PromotionRepository(), new DepositRepository(),
        new BookingRepository());

    private Credentials _credentials;
    private LoginDto _loginDto;
    private PromotionDto _promotionDto;
    private RegisterDto _registerDto;

    [TestInitialize]
    public void Initialize()
    {
        _app = new DepoQuickApp(new UserRepository(), new PromotionRepository(), new DepositRepository(),
            new BookingRepository());

        _registerDto = new RegisterDto
        {
            NameSurname = "Name Surname",
            Email = "test@test.com",
            Password = "12345678@mE",
            PasswordConfirmation = "12345678@mE",
            Rank = "Administrator"
        };

        _loginDto = new LoginDto
        {
            Email = "test@test.com",
            Password = "12345678@mE"
        };

        _credentials = new Credentials
        {
            Email = "test@test.com",
            Rank = "Administrator"
        };

        _addPromotionDto = new PromotionDto
        {
            Label = "Test Promotion",
            Discount = 10,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        _promotionDto = new PromotionDto
        {
            Id = 1,
            Label = "Test Promotion",
            Discount = 20,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        _depositDto = new DepositDto
        {
            Name = "Deposit",
            Area = "A",
            Size = "Small",
            ClimateControl = true,
            PromotionList = new List<int> { 1 }
        };

        _dateRangeDto = new DateRangeDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(100))
        };

        _bookingDto = new BookingDto
        {
            DepositName = _depositDto.Name,
            Email = _credentials.Email,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
    }

    [TestMethod]
    public void TestCanRegisterAndLoginUser()
    {
        // Act
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);

        Assert.AreEqual(_credentials.Email, credentials.Email);
        Assert.AreEqual(_credentials.Rank, credentials.Rank);
    }

    [TestMethod]
    public void TestCanAddPromotion()
    {
        //Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);

        // Act
        _app.AddPromotion(_addPromotionDto, credentials);

        // Assert
        var promotion = _app.GetPromotion(1);
        Assert.AreEqual(_addPromotionDto.Label, promotion.Label);
    }

    [TestMethod]
    public void TestCanDeletePromotion()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);

        // Act
        _app.DeletePromotion(1, credentials);
        var promotions = _app.ListAllPromotions(_credentials);

        // Assert
        Assert.AreEqual(0, promotions.Count());
    }

    [TestMethod]
    public void TestCanModifyPromotion()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);

        // Act
        _app.ModifyPromotion(1, _promotionDto, credentials);
        var promotion = _app.ListAllPromotions(_credentials).First(p => p.Id == 1);

        // Assert
        Assert.AreEqual(_promotionDto.Label, promotion.Label);
        Assert.AreEqual(_promotionDto.Discount, promotion.Discount);
        Assert.AreEqual(_promotionDto.DateFrom, promotion.DateFrom);
        Assert.AreEqual(_promotionDto.DateTo, promotion.DateTo);
    }

    [TestMethod]
    public void TestCanAddDeposit()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);

        // Act
        _app.AddDeposit(_depositDto, credentials);
        var deposit = _app.ListAllDeposits()[0];

        // Assert
        Assert.AreEqual(_depositDto.Name, deposit.Name);
        Assert.AreEqual(_depositDto.Area, deposit.Area);
        Assert.AreEqual(_depositDto.Size, deposit.Size);
        Assert.AreEqual(_depositDto.ClimateControl, deposit.ClimateControl);
        Assert.AreEqual(_depositDto.PromotionList[0], deposit.PromotionList[0]);
        Assert.AreEqual(_depositDto.PromotionList.Count, deposit.PromotionList.Count);
    }

    [TestMethod]
    public void TestCanDeleteDeposit()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_depositDto, credentials);

        // Act
        _app.DeleteDeposit(_depositDto.Name, credentials);
        var deposits = _app.ListAllDeposits();

        // Assert
        Assert.AreEqual(0, deposits.Count);
    }

    [TestMethod]
    public void TestCanAddBooking()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_depositDto, credentials);
        _app.AddAvailabilityPeriod(_depositDto.Name, _dateRangeDto, credentials);

        // Act
        _app.AddBooking(_bookingDto, credentials);
        var booking = _app.ListAllBookings(_credentials).Find(b => b.Id == 1);

        // Assert
        Assert.IsNotNull(booking);
        Assert.AreEqual(_bookingDto.DepositName, booking.DepositName);
        Assert.AreEqual(_bookingDto.Email, booking.Email);
        Assert.AreEqual(_bookingDto.DateFrom, booking.DateFrom);
        Assert.AreEqual(_bookingDto.DateTo, booking.DateTo);
    }

    [TestMethod]
    public void TestCanApproveBooking()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_depositDto, credentials);
        _app.AddAvailabilityPeriod(_depositDto.Name, _dateRangeDto, credentials);
        _app.AddBooking(_bookingDto, credentials);

        // Act
        _app.ApproveBooking(1, credentials);
        var booking = _app.ListAllBookings(_credentials).Find(b => b.Id == 1);

        // Assert
        Assert.AreEqual("Approved", booking?.Stage);
    }

    [TestMethod]
    public void TestCanRejectBooking()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_depositDto, credentials);
        _app.AddAvailabilityPeriod(_depositDto.Name, _dateRangeDto, credentials);
        _app.AddBooking(_bookingDto, credentials);

        // Act
        _app.RejectBooking(1, "Rejected", credentials);
        var booking = _app.ListAllBookings(_credentials).Find(b => b.Id == 1);

        // Assert
        Assert.AreEqual("Rejected", booking?.Stage);
        Assert.AreEqual("Rejected", booking?.Message);
    }

    [TestMethod]
    public void TestCantDeleteDepositIncludedInBookings()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_depositDto, credentials);
        _app.AddAvailabilityPeriod(_depositDto.Name, _dateRangeDto, credentials);
        _app.AddBooking(_bookingDto, credentials);

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => _app.DeleteDeposit(_depositDto.Name, credentials));

        // Assert
        Assert.AreEqual("There are existing bookings for this deposit.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeletePromotionIncludedInDeposits()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_depositDto, credentials);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => _app.DeletePromotion(1, credentials));

        // Assert
        Assert.AreEqual("There are existing deposits for this promotion.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingIfUserDoesNotExist()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_depositDto, credentials);
        var wrongBookingDto = new BookingDto
        {
            DepositName = "Deposit",
            Email = "wrong@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            _app.AddBooking(wrongBookingDto, credentials);
        });

        // Assert
        Assert.AreEqual("User not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingIfDepositDoesNotExist()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { _app.AddBooking(_bookingDto, credentials); });

        // Assert
        Assert.AreEqual("Deposit not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateDepositIfPromotionDoesNotExist()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        var wrongAddDepositDto = new DepositDto
        {
            Area = "A",
            Size = "Small",
            ClimateControl = true,
            PromotionList = new List<int> { 1 }
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            _app.AddDeposit(wrongAddDepositDto, credentials);
        });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Promotion not found."));
    }

    [TestMethod]
    public void TestCanGetDepositById()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_depositDto, credentials);

        // Act
        var deposit = _app.GetDeposit(_depositDto.Name);

        // Assert
        Assert.AreEqual(_depositDto.Area, deposit.Area);
        Assert.AreEqual(_depositDto.Size, deposit.Size);
        Assert.AreEqual(_depositDto.ClimateControl, deposit.ClimateControl);
        Assert.AreEqual(_depositDto.PromotionList[0], deposit.PromotionList[0]);
        Assert.AreEqual(_depositDto.PromotionList.Count, deposit.PromotionList.Count);
    }

    [TestMethod]
    public void TestCanGetBookingById()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_depositDto, credentials);
        _app.AddAvailabilityPeriod(_depositDto.Name, _dateRangeDto, credentials);
        _app.AddBooking(_bookingDto, credentials);

        // Act
        var booking = _app.GetBooking(1);

        // Assert
        Assert.IsNotNull(booking);
        Assert.AreEqual(_bookingDto.DepositName, booking.DepositName);
        Assert.AreEqual(_bookingDto.Email, booking.Email);
        Assert.AreEqual(_bookingDto.DateFrom, booking.DateFrom);
        Assert.AreEqual(_bookingDto.DateTo, booking.DateTo);
    }

    [TestMethod]
    public void TestCanListBookingsByEmail()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_depositDto, credentials);
        _app.AddAvailabilityPeriod(_depositDto.Name, _dateRangeDto, credentials);
        _app.AddBooking(_bookingDto, credentials);

        // Act
        var bookings = _app.ListAllBookingsByEmail(credentials.Email, credentials);

        // Assert
        Assert.AreEqual(1, bookings.Count);
        Assert.AreEqual(_bookingDto.DepositName, bookings[0].DepositName);
    }

    [TestMethod]
    public void TestCanAddAvailabilityPeriod()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_depositDto, credentials);

        // Act
        _app.AddAvailabilityPeriod(_depositDto.Name, _dateRangeDto, credentials);
        var deposit = _app.GetDeposit(_depositDto.Name);

        // Assert
        Assert.AreEqual(1, deposit.AvailabilityPeriods.Count);
    }
}