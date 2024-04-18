using BusinessLogic.Exceptions;

namespace BusinessLogic.Test;

[TestClass]
public class CredentialsManagerTest
{
    private const string Email = "test@test.com";
    private const string Password = "12345678@mE";
    
    
    [TestMethod]
    public void TestCanRegisterWithValidCredentials()
    {
        // Arrange
        var credManager = new CredentialsManager();

        // Act
        var credentials = credManager.Register(Email, Password, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        var credManager = new CredentialsManager();
        credManager.Register(Email, Password, Password);

        // Act
        var credentials = credManager.Login(Email, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCantRegisterWithSameEmail()
    {
        // Arrange
        var credManager = new CredentialsManager();
        credManager.Register(Email, Password, Password);

        // Act & Assert
        Assert.ThrowsException<UserAlreadyExistsException>(() =>
        {
            credManager.Register(Email, Password, Password);
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
            credManager.Register(Email, Password, "wrong");
        });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Passwords do not match."));
    }

    [TestMethod]
    public void TestCantLoginWithWrongPassword()
    {
        // Arrange
        var credManager = new CredentialsManager();
        credManager.Register(Email, Password, Password);

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => { credManager.Login(Email, "wrong"); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Wrong password."));
    }

    [TestMethod]
    public void TestCantLoginWithNonExistingUser()
    {
        // Arrange
        var credManager = new CredentialsManager();

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            credManager.Login(Email, Password);
        });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }
}