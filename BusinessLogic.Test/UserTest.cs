namespace BusinessLogic.Test;

[TestClass]
public class UserTest
{
    [TestMethod]
    public void TestCanCreateUserWithValidData()
    {
        // Act
        var user = new User("Name Surname", "test@test.com", "12345678@mE");

        // Assert
        Assert.IsNotNull(user);
    }

    [TestMethod]
    public void TestCantCreateUserWithInvalidEmailFormat()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            new User("Name Surname", "test", "12345678@mE");
        });

        // Assert
        Assert.AreEqual("Email format is invalid.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithAPasswordWithoutSymbols()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            new User("Name Surname", "test@test.com", "12345678mE");
        });

        // Assert
        Assert.AreEqual("Password format is invalid, it must contain at least one of the following symbols: #@$.,%",
            exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithPasswordLengthLessThan8()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            new User("Name Surname", "test@test.com", "1234@mE");
        });
        
        // Assert
        Assert.AreEqual("Password format is invalid, length must be at least 8.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithAPasswordWithoutUppercaseLetter()
    {
        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            new User("Name Surname", "test@test.com", "12345678@m");
        });
        
        // Assert
        Assert.AreEqual("Password format is invalid, it must contain at least one uppercase letter.", exception.Message);
    }
    
    [TestMethod]
    public void TestCantCreateUserWithAPasswordWithoutLowercaseLetter()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            new User("Name Surname", "test@test.com", "12345678@E");
        });

        // Assert
        Assert.AreEqual("Password format is invalid, it must contain at least one lowercase letter.", exception.Message);
    }
}