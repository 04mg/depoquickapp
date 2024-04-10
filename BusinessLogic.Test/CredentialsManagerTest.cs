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
}
