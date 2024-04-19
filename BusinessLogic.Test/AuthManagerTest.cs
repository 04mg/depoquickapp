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
        var userModel = new UserModel(NameSurname, Email, Password);
        
        // Act
        var credentials = credManager.Register(userModel, Password);

        // Assert
        Assert.AreSame(credentials.Email, Email);
    }

    [TestMethod]
    public void TestCanLoginWithValidCredentials()
    {
        // Arrange
        var credManager = new AuthManager();
        var userModel = new UserModel(NameSurname, Email, Password);
        credManager.Register(userModel, Password);

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
        var userModel = new UserModel(NameSurname, Email, Password);
        var otherUserModel = new UserModel("Other Name", Email, "OtherP@ssw0rd");
        credManager.Register(userModel, Password);

        // Act & Assert
        Assert.ThrowsException<UserAlreadyExistsException>(() => { credManager.Register(otherUserModel, Password); });
    }

    [TestMethod]
    public void TestCantRegisterWithNonMatchingPasswords()
    {
        // Arrange
        var credManager = new AuthManager();
        var userModel = new UserModel(NameSurname, Email, Password);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { credManager.Register(userModel, "wrong"); });

        // Assert
        Assert.IsTrue(exception.Message.Contains("Passwords do not match."));
    }

    [TestMethod]
    public void TestCantLoginWithWrongPassword()
    {
        // Arrange
        var credManager = new AuthManager();
        var userModel = new UserModel(NameSurname, Email, Password);
        credManager.Register(userModel, Password);

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
        var exception = Assert.ThrowsException<ArgumentException>(() => { credManager.Login(Email, Password); });

        // Assert
        Assert.AreSame(exception.Message, "User does not exist.");
    }

    [TestMethod]
    public void TestCantRegisterMoreThanOneAdmin()
    {
        // Arrange
        var credManager = new AuthManager();
        var userModel = new UserModel(NameSurname, Email, Password, UserRank.Administrator);
        var otherUserModel = new UserModel("Other Name", "test2@test.com", Password, UserRank.Administrator);
        
        // Act
        credManager.Register(userModel, Password);
        var exception = Assert.ThrowsException<ArgumentException>(() => { credManager.Register(otherUserModel, Password); });
        
        // Assert
        Assert.AreSame(exception.Message, "Auth error, There can be only one Administrator.");
    }
}