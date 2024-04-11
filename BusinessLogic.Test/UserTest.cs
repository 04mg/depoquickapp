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
}