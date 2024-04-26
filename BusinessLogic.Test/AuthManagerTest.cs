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
        var client = new User(NameSurname, Email, Password);

        // Act
        var credentials = credManager.Register(client, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        var credManager = new AuthManager();
        var client = new User(NameSurname, Email, Password);
        credManager.Register(client, Password);

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
        var client = new User(NameSurname, Email, Password);
        var otherClient = new User("Other Name", Email, "OtherP@ssw0rd");

        credManager.Register(client, Password);

        // Act & Assert
        Assert.ThrowsException<UserAlreadyExistsException>(() => { credManager.Register(otherClient, "OtherP@ssw0rd"); });
    }

    [TestMethod]
    public void TestCantRegisterWithNonMatchingPasswords()
    {
        // Arrange
        var credManager = new AuthManager();
        var client = new User(NameSurname, Email, Password);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { credManager.Register(client, "wrong"); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Passwords do not match."));
    }

    [TestMethod]
    public void TestCantLoginWithWrongPassword()
    {
        // Arrange
        var credManager = new AuthManager();
        var client = new User(NameSurname, Email, Password);
        credManager.Register(client, Password);

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
        var admin = new User(
            NameSurname, 
            Email, 
            Password, 
            "Administrator"
            );
        var otherAdmin = new User(
            "Other Name",
            "test2@test.com",
            Password,
            "Administrator"
            );

        // Act
        credManager.Register(admin, Password);
        var exception = Assert.ThrowsException<ArgumentException>(() => { credManager.Register(otherAdmin, Password); });

        // Assert
        Assert.AreSame(exception.Message, "There can only be one administrator.");
    }
    
    [TestMethod]
    public void TestCanCheckIfUserExists()
    {
        // Arrange
        var credManager = new AuthManager();
        var client = new User(NameSurname, Email, Password);

        credManager.Register(client, Password);

        // Act
        var userExists = credManager.Exists(Email);

        // Assert
        Assert.IsTrue(userExists);
    }
}