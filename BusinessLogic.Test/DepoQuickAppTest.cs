namespace BusinessLogic.Test;

[TestClass]
public class DepoQuickAppTest
{
    private DepoQuickApp _app;
    private RegisterDto _registerDto;
    private LoginDto _loginDto;
    private Credentials _credentials;
    private AuthManager _authManager;

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
}