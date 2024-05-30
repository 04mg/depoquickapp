using BusinessLogic.Calculators;
using BusinessLogic.Domain;

namespace BusinessLogic.Test;

[TestClass]
public class BookingTest
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Now);
    private static readonly DateOnly Tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

    private static readonly User Client = new(
        "Name Surname",
        "client@client.com",
        "12345678@mE"
    );

    private static readonly List<Promotion> Promotions = new()
    {
        new Promotion(1, "label", 50, Today, Tomorrow)
    };

    private Deposit _deposit = new("Deposit", "A", "Small", true, Promotions);

    [TestInitialize]
    public void Initialize()
    {
        _deposit = new Deposit("Deposit", "A", "Small", true, Promotions);
        _deposit.AddAvailabilityPeriod(new DateRange(Today, Today.AddDays(100)));
    }

    [TestMethod]
    public void TestCanCreateBookingWithValidData()
    {
        // Act
        var booking = new Booking(1, _deposit, Client, Today, Tomorrow);

        // Assert
        Assert.IsNotNull(booking);
    }

    [TestMethod]
    public void TestCantCreateBookingWithDateFromGreaterThanDateTo()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                new Booking(1, _deposit, Client, Tomorrow, Today));

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
                new Booking(1, _deposit, Client, Today.AddDays(-1), Tomorrow));

        // Assert
        Assert.AreEqual("The starting date of the booking must not be earlier than today.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingWithSameDateFromAndDateTo()
    {
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                new Booking(1, _deposit, Client, Today, Today));

        // Assert
        Assert.AreEqual("The starting date of the booking must not be the same as the ending date.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingIfTheDurationOfTheBookingIsNotIncludedInAnAvailabilityPeriod()
    {
        var depositA = new Deposit("Deposit", "A", "Small", true, Promotions);
        // Act
        var exception =
            Assert.ThrowsException<ArgumentException>(() =>
                new Booking(1, depositA, Client, Tomorrow, Tomorrow.AddDays(2)));
        // Assert
        Assert.AreEqual("The duration of the booking must be contained in the deposit availability periods.",
            exception.Message);
    }

    [TestMethod]
    public void TestCreatingABookingMakesHisDurationUnavailable()
    {
        // Act
        var booking = new Booking(1, _deposit, Client, Today, Tomorrow);

        // Assert
        var isAvailable = _deposit.IsAvailable(new DateRange(Today, Tomorrow));
        Assert.IsFalse(isAvailable);
    }

    [TestMethod]
    public void TestRejectingABookingMakesTheDurationAvailable()
    {
        // Arrange
        var booking = new Booking(1, _deposit, Client, Today, Tomorrow);

        // Act
        booking.Reject("rejection");

        // Assert
        Assert.IsTrue(_deposit.IsAvailable(new DateRange(Today, Tomorrow)));
    }

    [TestMethod]
    public void TestCantAddAvailabilityPeriodIfPeriodIsBooked()
    {
        // Arrange
        var availabilityPeriod = new DateRange(Today, Tomorrow);
        _deposit.AddAvailabilityPeriod(availabilityPeriod);
        var booking = new Booking(1, _deposit, Client, Today, Tomorrow);
        var overlappingPeriod = new DateRange(Today, Tomorrow.AddDays(1));

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _deposit.AddAvailabilityPeriod(overlappingPeriod));

        // Assert
        Assert.AreEqual($"The availability period overlaps with an already booked period from {Today} to {Tomorrow}.",
            exception.Message);
    }
}