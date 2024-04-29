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
}