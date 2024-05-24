using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Managers;

namespace BusinessLogic.Test;

[TestClass]
public class AuthControllerTest
{
    private const string NameSurname = "Name Surname";
    private const string Email = "test@test.com";
    private const string Password = "12345678@mE";
    private User? _client;

    private User Client
    {
        get => _client ?? throw new NullReferenceException("Client is not initialized.");
        set => _client = value;
    }

    [TestInitialize]
    public void SetUp()
    {
        Client = new User(NameSurname, Email, Password);
    }

    [TestMethod]
    public void TestCanRegisterWithValidCredentials()
    {
        // Arrange
        var authController = new AuthController();

        // Act
        var credentials = authController.Register(Client, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        var authController = new AuthController();
        authController.Register(Client, Password);

        // Act
        var credentials = authController.Login(new LoginDto { Email = Email, Password = Password });

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCantRegisterWithSameEmail()
    {
        // Arrange
        var authController = new AuthController();
        var otherClient = new User("Other Name", Email, "OtherP@ssw0rd");

        authController.Register(Client, Password);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => { authController.Register(otherClient, "OtherP@ssw0rd"); });
    }

    [TestMethod]
    public void TestCantRegisterWithNonMatchingPasswords()
    {
        // Arrange
        var authController = new AuthController();

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { authController.Register(Client, "wrong"); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Passwords do not match."));
    }

    [TestMethod]
    public void TestCantLoginWithWrongPassword()
    {
        // Arrange
        var authController = new AuthController();
        authController.Register(Client, Password);

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
            {
                authController.Login(new LoginDto { Email = Email, Password = "wrong" });
            });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Wrong password."));
    }

    [TestMethod]
    public void TestCantLoginWithNonExistingUser()
    {
        // Arrange
        var authController = new AuthController();

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            authController.Login(new LoginDto { Email = Email, Password = Password });
        });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }

    [TestMethod]
    public void TestCantRegisterMoreThanOneAdmin()
    {
        // Arrange
        var authController = new AuthController();
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
        authController.Register(admin, Password);
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            authController.Register(otherAdmin, Password);
        });

        // Assert
        Assert.AreSame(exception.Message, "There can only be one administrator.");
    }

    [TestMethod]
    public void TestCantGetUserByEmailIfUserDoesNotExist()
    {
        // Arrange
        var authController = new AuthController();
        var credentials = new Credentials
        {
            Email = "test@test.com",
            Rank = "Administrator"
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            authController.GetUser(Email, credentials);
        });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }

    [TestMethod]
    public void TestCantGetUserByEmailOfAnotherUserIfNotAdministrator()
    {
        // Arrange
        var authController = new AuthController();
        authController.Register(Client, Password);
        var otherClient = new User(
            "Other Name",
            "other@test.com",
            "OtherP@ssw0rd");
        authController.Register(otherClient, "OtherP@ssw0rd");
        var loginDto = new LoginDto
        {
            Email = otherClient.Email,
            Password = otherClient.Password
        };
        var credentials = authController.Login(loginDto);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
        {
            authController.GetUser(Email, credentials);
        });

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestFirstUserIsAdmin()
    {
        // Arrange
        var authController = new AuthController();
        var admin = new User(
            NameSurname,
            Email,
            Password
        );

        // Act
        var credentials = authController.Register(admin, Password);

        // Assert
        Assert.AreEqual("Administrator", credentials.Rank);
    }
}