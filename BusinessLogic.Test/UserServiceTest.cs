using BusinessLogic.DTOs;
using BusinessLogic.Services;
using DataAccess;

namespace BusinessLogic.Test;

[TestClass]
public class UserServiceTest
{
    private const string NameSurname = "Name Surname";
    private const string Email = "test@test.com";
    private const string Password = "12345678@mE";

    private readonly LoginDto _loginDto = new()
    {
        Email = Email,
        Password = Password
    };

    private readonly RegisterDto _registerDto = new()
    {
        NameSurname = NameSurname,
        Email = Email,
        Password = Password,
        PasswordConfirmation = Password,
        Rank = "Client"
    };

    private UserService _userService = new(new UserRepository());

    [TestInitialize]
    public void Initialize()
    {
        _userService = new UserService(new UserRepository());
    }

    [TestMethod]
    public void TestCanRegisterWithValidCredentials()
    {
        // Act
        var credentials = _userService.Register(_registerDto);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        _userService.Register(_registerDto);

        // Act
        var credentials = _userService.Login(_loginDto);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCantRegisterWithSameEmail()
    {
        // Arrange
        _userService.Register(_registerDto);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => { _userService.Register(_registerDto); });
    }

    [TestMethod]
    public void TestCantRegisterWithNonMatchingPasswords()
    {
        // Arrange
        var wrongRegisterDto = new RegisterDto
        {
            NameSurname = NameSurname,
            Email = Email,
            Password = Password,
            PasswordConfirmation = "wrong",
            Rank = "Client"
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { _userService.Register(wrongRegisterDto); });

        // Assert
        Assert.AreEqual("Passwords do not match.", exception.Message);
    }

    [TestMethod]
    public void TestCantLoginWithWrongPassword()
    {
        // Arrange
        _userService.Register(_registerDto);
        var wrongLoginDto = new LoginDto
        {
            Email = Email,
            Password = "wrong"
        };

        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => { _userService.Login(wrongLoginDto); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Wrong password."));
    }

    [TestMethod]
    public void TestCantLoginWithNonExistingUser()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { _userService.Login(_loginDto); });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }

    [TestMethod]
    public void TestCantRegisterMoreThanOneAdmin()
    {
        // Arrange
        var adminRegisterDto = new RegisterDto
        {
            NameSurname = NameSurname,
            Email = Email,
            Password = Password,
            PasswordConfirmation = Password,
            Rank = "Administrator"
        };
        var otherAdminRegisterDto = new RegisterDto
        {
            NameSurname = "Another Name",
            Email = "another@test.com",
            Password = Password,
            PasswordConfirmation = Password,
            Rank = "Administrator"
        };

        // Act
        _userService.Register(adminRegisterDto);
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            _userService.Register(otherAdminRegisterDto);
        });

        // Assert
        Assert.AreSame(exception.Message, "There can only be one administrator.");
    }
    
    [TestMethod]
    public void TestFirstUserIsAdmin()
    {
        // Act
        var credentials = _userService.Register(_registerDto);

        // Assert
        Assert.AreEqual("Administrator", credentials.Rank);
    }
}