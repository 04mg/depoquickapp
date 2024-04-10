using BusinessLogic.Exceptions;

namespace BusinessLogic.Test;

[TestClass]
public class CredentialsManagerTest
{
    [TestMethod]
    public void TestCanRegisterWithValidCredentials()
    {
        // Arrange
        var credManager = new CredentialsManager();

        // Act
        credManager.Register("test@test.com", "12345678@mE", "12345678@mE");

        // Assert
        Assert.IsTrue(credManager.IsRegistered("test@test.com"));
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        var credManager = new CredentialsManager();
        credManager.Register("test@test.com", "12345678@mE", "12345678@mE");

        // Act
        var credentials = credManager.Login("test@test.com", "12345678@mE");

        // Assert
        Assert.AreSame(credentials.Email, "test@test.com");
    }

    [TestMethod]
    public void TestCantRegisterWithSameEmail()
    {
        // Arrange
        var credManager = new CredentialsManager();
        credManager.Register("test@test.com", "12345678@mE", "12345678@mE");

        // Act & Assert
        Assert.ThrowsException<UserAlreadyExistsException>(() =>
        {
            credManager.Register("test@test.com", "12345678@mE", "12345678@mE");
        });
    }

    [TestMethod]
    public void TestCantRegisterWithNonMatchingPasswords()
    {
        // Arrange
        var credManager = new CredentialsManager();

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            credManager.Register("test@test.com", "12345678@mE", "wrong");
        });
        
        // Assert
        Assert.IsTrue(exception.Message.Contains("Passwords do not match."));
    }

    [TestMethod]
    public void TestCantLoginWithWrongPassword()
    {
        // Arrange
        var credManager = new CredentialsManager();
        credManager.Register("test@test.com", "12345678@mE", "12345678@mE");

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            credManager.Login("test@test.com", "wrong");
        });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Wrong password."));
    }
}