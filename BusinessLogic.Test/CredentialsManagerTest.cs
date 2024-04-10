namespace BusinessLogic.Test;

[TestClass]
public class CredentialsManagerTest
{
    [TestMethod]
    public void TestCanRegisterWithValidCredentials()
    {
        // Arrange
        var credsManager = new CredentialsManager();

        // Act
        credsManager.Register("test@test.com", "12345678@mE", "12345678@mE");

        // Assert
        Assert.IsTrue(credsManager.IsRegistered("test@test.com"));
    }
    
    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        var credsManager = new CredentialsManager();
        
        // Act
        credsManager.Register("test@test.com", "12345678@mE", "12345678@mE");
        var credentials = credsManager.Login("test@test.com", "12345678@mE");
        
        // Assert
        Assert.AreSame(credentials.Email, "test@test.com");
    }
}
