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
        var userModel = new RegisterDto(NameSurname, Email, Password, Password);

        // Act
        var credentials = credManager.Register(userModel);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        var credManager = new AuthManager();
        var userModel = new RegisterDto(NameSurname, Email, Password, Password);
        credManager.Register(userModel);

        // Act
        var credentials = credManager.Login(new LoginDto() { Email = Email, Password = Password });

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCantRegisterWithSameEmail()
    {
        // Arrange
        var credManager = new AuthManager();
        var userModel = new RegisterDto(NameSurname, Email, Password, Password);
        var otherUserModel = new RegisterDto("Other Name", Email, "OtherP@ssw0rd", "OtherP@ssw0rd");
        credManager.Register(userModel);

        // Act & Assert
        Assert.ThrowsException<UserAlreadyExistsException>(() => { credManager.Register(otherUserModel); });
    }

    [TestMethod]
    public void TestCantRegisterWithNonMatchingPasswords()
    {
        // Arrange
        var credManager = new AuthManager();
        var userModel = new RegisterDto(NameSurname, Email, Password, "wrong");

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { credManager.Register(userModel); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Passwords do not match."));
    }

    [TestMethod]
    public void TestCantLoginWithWrongPassword()
    {
        // Arrange
        var credManager = new AuthManager();
        var userModel = new RegisterDto(NameSurname, Email, Password, Password);
        credManager.Register(userModel);

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
            {
                credManager.Login(new LoginDto() { Email = Email, Password = "wrong" });
            });

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
            credManager.Login(new LoginDto() { Email = Email, Password = Password });
        });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }

    [TestMethod]
    public void TestCantRegisterMoreThanOneAdmin()
    {
        // Arrange
        var credManager = new AuthManager();
        var userModel = new RegisterDto(NameSurname, Email, Password, Password, "Administrator");
        var otherUserModel = new RegisterDto("Other Name", "test2@test.com", Password, Password, "Administrator");

        // Act
        credManager.Register(userModel);
        var exception = Assert.ThrowsException<ArgumentException>(() => { credManager.Register(otherUserModel); });

        // Assert
        Assert.AreSame(exception.Message, "There can only be one administrator.");
    }
}