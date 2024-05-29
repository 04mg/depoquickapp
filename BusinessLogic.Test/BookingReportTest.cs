using BusinessLogic.Calculators;
using BusinessLogic.Domain;

namespace BusinessLogic.Test;

[TestClass]
public class BookingReportTest
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
        new(1, "label", 50, Today, Tomorrow)
    };

    private Deposit _deposit;

    [TestInitialize]
    public void Initialize()
    {
        _deposit = new Deposit("Deposit", "A", "Small", true, Promotions);
        _deposit.AddAvailabilityPeriod(new DateRange(Today, Today.AddDays(100)));
    }

    [TestMethod]
    public void TestCanGenerateTxtBookingReportContent()
    {
        // Arrange
        var booking = new Booking(1, _deposit, Client, Today, Tomorrow, new PriceCalculator());
        var bookingReport = new BookingReport(booking);

        // Act
        var reportContent = bookingReport.GenerateTxtReportContent();

        // Assert
        Assert.AreEqual("Deposit\t" +
                        $"{Today}-{Tomorrow}\t" +
                        "client@client.com\t" +
                        "35$\t" +
                        "Yes\n", reportContent);
    }
}