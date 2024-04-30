namespace BusinessLogic.Test;

[TestClass]
public class DepoQuickAppTest
{
    private DepoQuickApp _app;
    private RegisterDto _registerDto;
    private LoginDto _loginDto;
    private Credentials _credentials;
    private AddPromotionDto _addPromotionDto;
    private ModifyPromotionDto _modifyPromotionDto;
    private AddDepositDto _addDepositDto;
    private AddBookingDto _addBookingDto;

    [TestInitialize]
    public void SetUp()
    {
        _app = new DepoQuickApp();
        _registerDto = new RegisterDto{
            NameSurname = "Name Surname", 
            Email = "test@test.com", 
            Password = "12345678@mE", 
            PasswordConfirmation = "12345678@mE", 
            Rank = "Administrator"};
        _loginDto = new LoginDto{
            Email = "test@test.com", 
            Password = "12345678@mE"};
        _credentials = new Credentials{
            Email = "test@test.com", 
            Rank = "Administrator"};
        _addPromotionDto = new AddPromotionDto
        {
            Label = "Test Promotion", 
            Discount = 10, 
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        _modifyPromotionDto = new ModifyPromotionDto
        {
            Id = 1,
            Label = "Test Promotion", 
            Discount = 20, 
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        _addDepositDto = new AddDepositDto
        {
            Area = "A", 
            Size = "Small", 
            ClimateControl = true, 
            PromotionList = new List<int>{1}
        };
        _addBookingDto = new AddBookingDto
        {
            DepositId = 1, 
            Email = "test@test.com",
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
        Assert.AreEqual(0, promotions.Count);
    }
    
    [TestMethod]
    public void TestCanModifyPromotion()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);

        // Act
        _app.ModifyPromotion(1, _modifyPromotionDto, credentials);
        var promotion = _app.ListAllPromotions(_credentials).First(p => p.Id == 1);

        // Assert
        Assert.AreEqual(_modifyPromotionDto.Label, promotion.Label);
        Assert.AreEqual(_modifyPromotionDto.Discount, promotion.Discount);
        Assert.AreEqual(_modifyPromotionDto.DateFrom, promotion.DateFrom);
        Assert.AreEqual(_modifyPromotionDto.DateTo, promotion.DateTo);
    }
    
    [TestMethod]
    public void TestCanAddDeposit()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);

        // Act
        _app.AddDeposit(_addDepositDto, credentials);
        var deposit = _app.ListAllDeposits(_credentials)[0];

        // Assert
        Assert.AreEqual(_addDepositDto.Area, deposit.Area);
        Assert.AreEqual(_addDepositDto.Size, deposit.Size);
        Assert.AreEqual(_addDepositDto.ClimateControl, deposit.ClimateControl);
        Assert.AreEqual(_addDepositDto.PromotionList[0], deposit.PromotionList[0]);
        Assert.AreEqual(_addDepositDto.PromotionList.Count, deposit.PromotionList.Count);
    }
    
    [TestMethod]
    public void TestCanDeleteDeposit()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_addDepositDto, credentials);

        // Act
        _app.DeleteDeposit(1, credentials);
        var deposits = _app.ListAllDeposits(_credentials);

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
        _app.AddDeposit(_addDepositDto, credentials);

        // Act
        _app.AddBooking(_addBookingDto, _credentials);
        var booking = _app.ListAllBookings(_credentials).Find(b => b.Id == 1);

        // Assert
        Assert.IsNotNull(booking);
        Assert.AreEqual(_addBookingDto.DepositId, booking.Id);
        Assert.AreEqual(_addBookingDto.Email, booking.Email);
        Assert.AreEqual(_addBookingDto.DateFrom, booking.DateFrom);
        Assert.AreEqual(_addBookingDto.DateTo, booking.DateTo);
    }
    
    [TestMethod]
    public void TestCanApproveBooking()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_addDepositDto, credentials);
        _app.AddBooking(_addBookingDto, _credentials);

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
        _app.AddDeposit(_addDepositDto, credentials);
        _app.AddBooking(_addBookingDto, _credentials);

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
        _app.AddDeposit(_addDepositDto, credentials);
        _app.AddBooking(_addBookingDto, _credentials);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => _app.DeleteDeposit(1, credentials));
        
        // Assert
        Assert.AreEqual("Cant delete deposit, it is included in bookings.", exception.Message);
    }

    [TestMethod]
    public void TestCantDeletePromotionIncludedInDeposits()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_addDepositDto, credentials);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => _app.DeletePromotion(1, credentials));
        
        // Assert
        Assert.AreEqual("Cant delete promotion, it is included in deposits.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingIfUserDoesNotExist()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        _app.AddPromotion(_addPromotionDto, credentials);
        _app.AddDeposit(_addDepositDto, credentials);
        var wrongBookingDto = new AddBookingDto
        {
            DepositId = 1,
            Email = "wrong@test.com",
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { _app.AddBooking(wrongBookingDto, _credentials); });

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
        var exception = Assert.ThrowsException<ArgumentException>(() => { _app.AddBooking(_addBookingDto, _credentials); });
        
        // Assert
        Assert.IsTrue(exception.Message.Contains("Deposit not found."));
    }
    
    [TestMethod]
    public void TestCantCreateDepositIfPromotionDoesNotExist()
    {
        // Arrange
        _app.RegisterUser(_registerDto);
        var credentials = _app.Login(_loginDto);
        var wrongAddDepositDto = new AddDepositDto
        {
            Area = "A",
            Size = "Small",
            ClimateControl = true,
            PromotionList = new List<int> {1}
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { _app.AddDeposit(wrongAddDepositDto, credentials); });
        
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
        _app.AddDeposit(_addDepositDto, credentials);

        // Act
        var deposit = _app.GetDeposit(1, credentials);

        // Assert
        Assert.AreEqual(_addDepositDto.Area, deposit.Area);
        Assert.AreEqual(_addDepositDto.Size, deposit.Size);
        Assert.AreEqual(_addDepositDto.ClimateControl, deposit.ClimateControl);
        Assert.AreEqual(_addDepositDto.PromotionList[0], deposit.PromotionList[0]);
        Assert.AreEqual(_addDepositDto.PromotionList.Count, deposit.PromotionList.Count);
    }
}