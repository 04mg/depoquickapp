namespace BusinessLogic.Test;

[TestClass]
public class BookingTest
{
    private const string Email = "test@test.com";
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);
    private readonly DateOnly _tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

    [TestMethod]
    public void TestCanCreateBookingWithValidData()
    {
        // Act
        var booking = new Booking(1, 1, Email, _today, _tomorrow);

        // Assert
        Assert.IsNotNull(booking);
    }

    [TestMethod]
    public void TestCantCreateBookingWithDateFromGreaterThanDateTo()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => new Booking(1, 1, Email, _tomorrow, _today));

        // Assert
        Assert.AreEqual("The starting date of the booking must not be later than the ending date.",
            exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingWithDateFromLessThanToday()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() => new Booking(1, 1, Email, _today.AddDays(-1), _tomorrow));

        // Assert
        Assert.AreEqual("The starting date of the booking must not be earlier than today.", exception.Message);
    }
}