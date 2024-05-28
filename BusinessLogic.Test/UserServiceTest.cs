using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Repositories;
using BusinessLogic.Services;

namespace BusinessLogic.Test;

[TestClass]
public class UserServiceTest
{
    private const string NameSurname = "Name Surname";
    private const string Email = "test@test.com";
    private const string Password = "12345678@mE";
    private User Client { get; set; } = new User(NameSurname, Email, Password);
    private IUserRepository UserRepository { get; set; } = new UserRepository();

    [TestInitialize]
    public void SetUp()
    {
        Client = new User(NameSurname, Email, Password);
        UserRepository = new UserRepository();
    }

    [TestMethod]
    public void TestCanRegisterWithValidCredentials()
    {
        // Arrange
        var authService = new UserService(UserRepository);

        // Act
        var credentials = authService.Register(Client, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        var authService = new UserService(UserRepository);
        authService.Register(Client, Password);

        // Act
        var credentials = authService.Login(Email, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCantRegisterWithSameEmail()
    {
        // Arrange
        var authService = new UserService(UserRepository);
        var otherClient = new User("Other Name", Email, "OtherP@ssw0rd");

        authService.Register(Client, Password);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => { authService.Register(otherClient, "OtherP@ssw0rd"); });
    }

    [TestMethod]
    public void TestCantRegisterWithNonMatchingPasswords()
    {
        // Arrange
        var authService = new UserService(UserRepository);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { authService.Register(Client, "wrong"); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Passwords do not match."));
    }

    [TestMethod]
    public void TestCantLoginWithWrongPassword()
    {
        // Arrange
        var authService = new UserService(UserRepository);
        authService.Register(Client, Password);

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => { authService.Login(Email, "wrong"); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Wrong password."));
    }

    [TestMethod]
    public void TestCantLoginWithNonExistingUser()
    {
        // Arrange
        var authService = new UserService(UserRepository);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { authService.Login(Email, Password); });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }

    [TestMethod]
    public void TestCantRegisterMoreThanOneAdmin()
    {
        // Arrange
        var authService = new UserService(UserRepository);
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
        authService.Register(admin, Password);
        var exception = Assert.ThrowsException<ArgumentException>(() => { authService.Register(otherAdmin, Password); });

        // Assert
        Assert.AreSame(exception.Message, "There can only be one administrator.");
    }

    [TestMethod]
    public void TestCantGetUserByEmailIfUserDoesNotExist()
    {
        // Arrange
        var authService = new UserService(UserRepository);
        var credentials = new Credentials
        {
            Email = "test@test.com",
            Rank = "Administrator"
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { authService.GetUser(Email, credentials); });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }

    [TestMethod]
    public void TestCantGetUserByEmailOfAnotherUserIfNotAdministrator()
    {
        // Arrange
        var authService = new UserService(UserRepository);
        authService.Register(Client, Password);
        var otherClient = new User(
            "Other Name",
            "other@test.com",
            "OtherP@ssw0rd");
        authService.Register(otherClient, "OtherP@ssw0rd");
        var credentials = authService.Login(otherClient.Email, otherClient.Password);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
        {
            authService.GetUser(Email, credentials);
        });

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestFirstUserIsAdmin()
    {
        // Arrange
        var authService = new UserService(UserRepository);
        var admin = new User(
            NameSurname,
            Email,
            Password
        );

        // Act
        var credentials = authService.Register(admin, Password);

        // Assert
        Assert.AreEqual("Administrator", credentials.Rank);
    }
}