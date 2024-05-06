using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Managers;

namespace BusinessLogic.Test;

[TestClass]
public class AuthManagerTest
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
        var credManager = new AuthManager();

        // Act
        var credentials = credManager.Register(Client, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        var credManager = new AuthManager();
        credManager.Register(Client, Password);

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
        var otherClient = new User("Other Name", Email, "OtherP@ssw0rd");

        credManager.Register(Client, Password);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => { credManager.Register(otherClient, "OtherP@ssw0rd"); });
    }

    [TestMethod]
    public void TestCantRegisterWithNonMatchingPasswords()
    {
        // Arrange
        var credManager = new AuthManager();

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { credManager.Register(Client, "wrong"); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Passwords do not match."));
    }

    [TestMethod]
    public void TestCantLoginWithWrongPassword()
    {
        // Arrange
        var credManager = new AuthManager();
        credManager.Register(Client, Password);

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

        credManager.Register(Client, Password);

        // Act
        var userExists = credManager.Exists(Email);

        // Assert
        Assert.IsTrue(userExists);
    }
    
    [TestMethod]
    public void TestCantGetUserByEmailIfUserDoesNotExist()
    {
        // Arrange
        var credManager = new AuthManager();
        var credentials = new Credentials()
        {
            Email = "test@test.com",
            Rank = "Administrator"
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { credManager.GetUserByEmail(Email, credentials); });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }

    [TestMethod]
    public void TestCantGetUserByEmailOfAnotherUserIfNotAdministrator()
    {
        // Arrange
        var credManager = new AuthManager();
        credManager.Register(Client, Password);
        var otherClient = new User(
            "Other Name", 
            "other@test.com", 
            "OtherP@ssw0rd");
        credManager.Register(otherClient, "OtherP@ssw0rd");
        var loginDto = new LoginDto()
        {
            Email = otherClient.Email,
            Password = otherClient.Password
        };
        var credentials = credManager.Login(loginDto);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() => { credManager.GetUserByEmail(Email, credentials); });

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestFirstUserIsAdmin()
    {
        // Arrange
        var credManager = new AuthManager();
        var admin = new User(
            NameSurname,
            Email,
            Password
            );

        // Act
        var credentials = credManager.Register(admin, Password);

        // Assert
        Assert.AreEqual("Administrator", credentials.Rank);
    }
}