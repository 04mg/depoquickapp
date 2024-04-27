namespace BusinessLogic.Test;

[TestClass]
public class BookingTest
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Now);
    private static readonly DateOnly Tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
    private static readonly User Client = new User(
        "Name Surname",
        "client@client.com",
        "12345678@mE"
    );
    private static readonly List<Promotion> Promotions = new List<Promotion>
    {
        new Promotion(1, "label", 50, Today, Tomorrow)
    };
    private static readonly Deposit Deposit = new Deposit(1, "A", "Small", true, Promotions);

    [TestMethod]
    public void TestCanCreateBookingWithValidData()
    {
        // Act
        var booking = new Booking(1, Deposit, Client, Today, Tomorrow);

        // Assert
        Assert.IsNotNull(booking);
    }

    [TestMethod]
    public void TestCantCreateBookingWithDateFromGreaterThanDateTo()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                new Booking(1, Deposit, Client, Tomorrow, Today));

        // Assert
        Assert.AreEqual("The starting date of the booking must not be later than the ending date.",
            exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingWithDateFromLessThanToday()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                new Booking(1, Deposit, Client, Today.AddDays(-1), Tomorrow));

        // Assert
        Assert.AreEqual("The starting date of the booking must not be earlier than today.", exception.Message);
    }
}