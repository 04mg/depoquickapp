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

    private Deposit? _deposit;
    private Deposit? _depositNoPromotions;

    [TestInitialize]
    public void Initialize()
    {
        _deposit = new Deposit("Deposit", "A", "Small", true, Promotions);
        _deposit.AddAvailabilityPeriod(new DateRange(Today, Today.AddDays(100)));

        _depositNoPromotions = new Deposit("Deposit", "A", "Small", true, new List<Promotion>());
        _depositNoPromotions.AddAvailabilityPeriod(new DateRange(Today, Today.AddDays(100)));
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists("BookingReport_1.txt"))
        {
            File.Delete("BookingReport_1.txt");
        }

        if (File.Exists("BookingReport_1.csv"))
        {
            File.Delete("BookingReport_1.csv");
        }
    }

    [TestMethod]
    public void TestCanGenerateTxtBookingReportContent()
    {
        // Arrange
        var booking = new Booking(1, _deposit, Client, Today, Tomorrow, new PriceCalculator());

        // Act
        var reportContent = BookingReport.GenerateTxtReportContent(booking);

        // Assert
        Assert.AreEqual("Deposit\t" +
                        $"{Today}-{Tomorrow}\t" +
                        "client@client.com\t" +
                        "35$\t" +
                        "Yes\n", reportContent);
    }

    [TestMethod]
    public void TestCanGenerateTxtBookingReportContentWithoutPromotions()
    {
        // Arrange
        var booking = new Booking(1, _depositNoPromotions, Client, Today, Tomorrow, new PriceCalculator());

        // Act
        var reportContent = BookingReport.GenerateTxtReportContent(booking);

        // Assert
        Assert.AreEqual("Deposit\t" +
                        $"{Today}-{Tomorrow}\t" +
                        "client@client.com\t" +
                        "70$\t" +
                        "No\n", reportContent);
    }

    [TestMethod]
    public void TestCanGenerateCsvBookingReportContent()
    {
        // Arrange
        var booking = new Booking(1, _deposit, Client, Today, Tomorrow, new PriceCalculator());

        // Act
        var reportContent = BookingReport.GenerateCsvReportContent(booking);

        // Assert
        Assert.AreEqual("Deposit,Client,StartDate,EndDate,Price,Confirmed\n" +
                        "Deposit,client@client.com," +
                        $"{Today:yyyy-MM-dd},{Tomorrow:yyyy-MM-dd},35$,Yes\n", reportContent);
    }

    [TestMethod]
    public void TestCanGenerateCsvBookingReportContentWithoutPromotions()
    {
        // Arrange
        var booking = new Booking(1, _depositNoPromotions, Client, Today, Tomorrow, new PriceCalculator());

        // Act
        var reportContent = BookingReport.GenerateCsvReportContent(booking);

        // Assert
        Assert.AreEqual("Deposit,Client,StartDate,EndDate,Price,Confirmed\n" +
                        "Deposit,client@client.com," +
                        $"{Today:yyyy-MM-dd},{Tomorrow:yyyy-MM-dd},70$,No\n", reportContent);
    }

    [TestMethod]
    public void TestCanCreateTxtBookingReportFile()
    {
        // Arrange
        var booking = new Booking(1, _deposit, Client, Today, Tomorrow, new PriceCalculator());
        const string path = $"BookingReport_1.txt";
        var reportContent = BookingReport.GenerateTxtReportContent(booking);

        // Act
        BookingReport.CreateTxtReportFile(booking);

        // Assert
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual(File.ReadAllText(path), reportContent);
    }

    [TestMethod]
    public void TestCanCreateCsvBookingReportFile()
    {
        // Arrange
        var booking = new Booking(1, _deposit, Client, Today, Tomorrow, new PriceCalculator());
        const string path = $"BookingReport_1.csv";
        var reportContent = BookingReport.GenerateCsvReportContent(booking);

        // Act
        BookingReport.CreateCsvReportFile(booking);

        // Assert
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual(File.ReadAllText(path), reportContent);
    }
}