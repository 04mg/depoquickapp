using BusinessLogic.DTOs;
using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using DataAccess;
using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

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

    private UserService _userService = null!;

    [TestInitialize]
    public void Initialize()
    {
        var testsContext = new ProgramTest();
        using var scope = testsContext.ServiceProvider.CreateScope();
        scope.ServiceProvider.GetRequiredService<UserRepository>();
        var userRepository = scope.ServiceProvider.GetRequiredService<UserRepository>();
        _userService = new UserService(userRepository);
    }

    [TestMethod]
    public void TestCanRegisterWithValidCredentials()
    {
        // Act
        _userService.Register(_registerDto);
        _userService.Login(_loginDto);

        // Assert
        Assert.AreSame(_userService.CurrentCredentials.Email, Email);
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        _userService.Register(_registerDto);

        // Act
        _userService.Login(_loginDto);

        // Assert
        Assert.IsTrue(_userService.IsLoggedIn());
    }

    [TestMethod]
    public void TestCantRegisterWithSameEmail()
    {
        // Arrange
        _userService.Register(_registerDto);

        // Act & Assert
        Assert.ThrowsException<BusinessLogicException>(() => { _userService.Register(_registerDto); });
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
        var exception = Assert.ThrowsException<BusinessLogicException>(() =>
        {
            _userService.Register(wrongRegisterDto);
        });

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
            Assert.ThrowsException<BusinessLogicException>(() => { _userService.Login(wrongLoginDto); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Wrong password."));
    }

    [TestMethod]
    public void TestCantLoginWithNonExistingUser()
    {
        // Act
        var exception = Assert.ThrowsException<BusinessLogicException>(() => { _userService.Login(_loginDto); });

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
        var exception = Assert.ThrowsException<BusinessLogicException>(() =>
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
        _userService.Register(_registerDto);
        _userService.Login(_loginDto);

        // Assert
        Assert.IsTrue(_userService.CurrentCredentials.Rank == "Administrator");
        Assert.IsTrue(_userService.IsAdmin());
    }
    
    [TestMethod]
    public void TestCanLogout()
    {
        // Arrange
        _userService.Register(_registerDto);
        _userService.Login(_loginDto);

        // Act
        _userService.Logout();

        // Assert
        Assert.IsFalse(_userService.IsLoggedIn());
    }
}