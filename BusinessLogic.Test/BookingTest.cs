namespace BusinessLogic.Test;

[TestClass]
public class BookingTest
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);
    private readonly DateOnly _tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

    [TestMethod]
    public void TestCanCreateBookingWithValidData()
    {
        // Act
        var booking = new Booking(1, 1, "test@test.com", _today, _tomorrow);

        // Assert
        Assert.IsNotNull(booking);
    }
}