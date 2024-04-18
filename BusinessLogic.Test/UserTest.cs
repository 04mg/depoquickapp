namespace BusinessLogic.Test;

[TestClass]
public class UserTest
{
    private String _nameSurname;
    private String _email;
    private String _password;

    [TestInitialize]
    public void TestInitialize()
    {
        _nameSurname = "Name Surname";
        _email = "test@test.com";
        _password = "12345678@mE";
    }

    [TestMethod]
    public void TestCanCreateUserWithValidData()
    {
        // Act
        var user = new User(_nameSurname, _email, _password);

        // Assert
        Assert.IsNotNull(user);
    }

    [TestMethod]
    public void TestCantCreateUserWithInvalidEmailFormat()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new User(_nameSurname, "test", _password));

        // Assert
        Assert.AreEqual("Email format is invalid.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithEmailLengthGreaterThan254()
    {
        // Arrange
        var emailLength255 = new string('a', 245) + "@gmail.com";
        
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new User(_nameSurname, emailLength255, _password));
        
        // Assert
        Assert.AreEqual("Email format is invalid, length must be lesser or equal to 254.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithAPasswordWithoutSymbols()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new User(_nameSurname, _email, "12345678mE"));

        // Assert
        Assert.AreEqual("Password format is invalid, it must contain at least one of the following symbols: #@$.,%",
            exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithPasswordLengthLessThan8()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new User(_nameSurname, _email, "1234@mE"));

        // Assert
        Assert.AreEqual("Password format is invalid, length must be at least 8.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithAPasswordWithoutUppercaseLetter()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            return new User(_nameSurname, _email, "12345678@m");
        });

        // Assert
        Assert.AreEqual("Password format is invalid, it must contain at least one uppercase letter.",
            exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithAPasswordWithoutLowercaseLetter()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new User(_nameSurname, _email, "12345678@E"));

        // Assert
        Assert.AreEqual("Password format is invalid, it must contain at least one lowercase letter.",
            exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithAPasswordWithoutADigit()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new User(_nameSurname, _email, "password@E"));

        // Assert
        Assert.AreEqual("Password format is invalid, it must contain at least one digit.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithANameSurnameWithoutSpace()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new User("NameSurname", _email, _password));

        // Assert
        Assert.AreEqual("NameSurname format is invalid, it has to contain a space between the name and surname.",
            exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithoutSurname()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new User("Name ", _email, _password));

        // Assert
        Assert.AreEqual("NameSurname format is invalid, it has to contain a name and a surname.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithoutName()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new User(" Surname", _email, _password));

        // Assert
        Assert.AreEqual("NameSurname format is invalid, it has to contain a name and a surname.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithNameSurnameLengthGreaterThan100()
    {
        // Arrange
        const string nameLength101 = "Name Surname Name Surname Name Surname Name Surname Name Surname Name Surname Name Surname Name Seven";
        
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new User(nameLength101, _email, _password));

        // Assert
        Assert.AreEqual("NameSurname format is invalid, length must be lesser or equal to 100.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateUserWithNameSurnameWithNumbers()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => new User("Name 123", _email, _password));
        
        // Assert
        Assert.AreEqual("NameSurname format is invalid, it should not contain numbers.", exception.Message);
    }

    [TestMethod]
    public void TestCanCreateAdminUserWithValidData()
    {
        // Arrange
        var user = new User(_nameSurname, _email, _password, UserRank.Administrator);

        // Assert
        Assert.AreEqual(UserRank.Administrator, user.Rank);
    }
}