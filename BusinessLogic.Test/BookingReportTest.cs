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
    
    [TestMethod]
    public void TestCanGenerateCsvBookingReportContent()
    {
        // Arrange
        var booking = new Booking(1, _deposit, Client, Today, Tomorrow, new PriceCalculator());
        var bookingReport = new BookingReport(booking);

        // Act
        var reportContent = bookingReport.GenerateCsvReportContent();

        // Assert
        Assert.AreEqual("Deposit,Client,StartDate,EndDate,Price,Confirmed\n" +
                        "Deposit,client@client.com," +
                        $"{Today:yyyy-MM-dd},{Tomorrow:yyyy-MM-dd},35$,Yes\n", reportContent);
    }

    [TestMethod]
    public void TestCanCreateTxtBookingReportFile()
    {
        // Arrange
        var booking = new Booking(1, _deposit, Client, Today, Tomorrow, new PriceCalculator());
        const string path = $"BookingReport_1.txt";
        var bookingReport = new BookingReport(booking);
        var reportContent = bookingReport.GenerateTxtReportContent();
        
        // Act
        bookingReport.CreateTxtReportFile();

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
        var bookingReport = new BookingReport(booking);
        var reportContent = bookingReport.GenerateCsvReportContent();
        
        // Act
        bookingReport.CreateCsvReportFile();

        // Assert
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual(File.ReadAllText(path), reportContent);
    }
}