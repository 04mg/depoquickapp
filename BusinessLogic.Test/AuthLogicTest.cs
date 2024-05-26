using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Logic;
using BusinessLogic.Repositories;

namespace BusinessLogic.Test;

[TestClass]
public class AuthLogicTest
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
        var authLogic = new AuthLogic(UserRepository);

        // Act
        var credentials = authLogic.Register(Client, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        var authLogic = new AuthLogic(UserRepository);
        authLogic.Register(Client, Password);

        // Act
        var credentials = authLogic.Login(Email, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCantRegisterWithSameEmail()
    {
        // Arrange
        var authLogic = new AuthLogic(UserRepository);
        var otherClient = new User("Other Name", Email, "OtherP@ssw0rd");

        authLogic.Register(Client, Password);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => { authLogic.Register(otherClient, "OtherP@ssw0rd"); });
    }

    [TestMethod]
    public void TestCantRegisterWithNonMatchingPasswords()
    {
        // Arrange
        var authLogic = new AuthLogic(UserRepository);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { authLogic.Register(Client, "wrong"); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Passwords do not match."));
    }

    [TestMethod]
    public void TestCantLoginWithWrongPassword()
    {
        // Arrange
        var authLogic = new AuthLogic(UserRepository);
        authLogic.Register(Client, Password);

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => { authLogic.Login(Email, "wrong"); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Wrong password."));
    }

    [TestMethod]
    public void TestCantLoginWithNonExistingUser()
    {
        // Arrange
        var authLogic = new AuthLogic(UserRepository);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { authLogic.Login(Email, Password); });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }

    [TestMethod]
    public void TestCantRegisterMoreThanOneAdmin()
    {
        // Arrange
        var authLogic = new AuthLogic(UserRepository);
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
        authLogic.Register(admin, Password);
        var exception = Assert.ThrowsException<ArgumentException>(() => { authLogic.Register(otherAdmin, Password); });

        // Assert
        Assert.AreSame(exception.Message, "There can only be one administrator.");
    }

    [TestMethod]
    public void TestCantGetUserByEmailIfUserDoesNotExist()
    {
        // Arrange
        var authLogic = new AuthLogic(UserRepository);
        var credentials = new Credentials
        {
            Email = "test@test.com",
            Rank = "Administrator"
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { authLogic.GetUser(Email, credentials); });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }

    [TestMethod]
    public void TestCantGetUserByEmailOfAnotherUserIfNotAdministrator()
    {
        // Arrange
        var authLogic = new AuthLogic(UserRepository);
        authLogic.Register(Client, Password);
        var otherClient = new User(
            "Other Name",
            "other@test.com",
            "OtherP@ssw0rd");
        authLogic.Register(otherClient, "OtherP@ssw0rd");
        var credentials = authLogic.Login(otherClient.Email, otherClient.Password);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
        {
            authLogic.GetUser(Email, credentials);
        });

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestFirstUserIsAdmin()
    {
        // Arrange
        var authLogic = new AuthLogic(UserRepository);
        var admin = new User(
            NameSurname,
            Email,
            Password
        );

        // Act
        var credentials = authLogic.Register(admin, Password);

        // Assert
        Assert.AreEqual("Administrator", credentials.Rank);
    }
}