using BusinessLogic.Exceptions;

namespace BusinessLogic.Test;

[TestClass]
public class AuthManagerTest
{
    private const string NameSurname = "Name Surname";
    private const string Email = "test@test.com";
    private const string Password = "12345678@mE";
    
    
    [TestMethod]
    public void TestCanRegisterWithValidCredentials()
    {
        // Arrange
        var credManager = new AuthManager();

        // Act
        var credentials = credManager.Register(NameSurname, Email, Password, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        var credManager = new AuthManager();
        credManager.Register(NameSurname, Email, Password, Password);

        // Act
        var credentials = credManager.Login(Email, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCantRegisterWithSameEmail()
    {
        // Arrange
        var credManager = new AuthManager();
        credManager.Register(NameSurname, Email, Password, Password);

        // Act & Assert
        Assert.ThrowsException<UserAlreadyExistsException>(() =>
        {
            credManager.Register(NameSurname, Email, Password, Password);
        });
    }

    [TestMethod]
    public void TestCantRegisterWithNonMatchingPasswords()
    {
        // Arrange
        var credManager = new AuthManager();

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            credManager.Register(NameSurname, Email, Password, "wrong");
        });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Passwords do not match."));
    }

    [TestMethod]
    public void TestCantLoginWithWrongPassword()
    {
        // Arrange
        var credManager = new AuthManager();
        credManager.Register(NameSurname, Email, Password, Password);

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
        var credManager = new AuthManager();

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            credManager.Login(Email, Password);
        });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }
}