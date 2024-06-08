using BusinessLogic.Reports;
using Calculators;
using Domain;
using Domain.Enums;

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
        new Promotion(1, "label", 50, Today, Tomorrow)
    };

    private Deposit? _deposit;
    private Deposit? _depositNoPromotions;

    [TestInitialize]
    public void Initialize()
    {
        _deposit = new Deposit("Deposit", DepositArea.A, DepositSize.Small, true, Promotions);
        _deposit.AddAvailabilityPeriod(new DateRange.DateRange(Today, Today.AddDays(100)));

        _depositNoPromotions = new Deposit("Deposit", DepositArea.A, DepositSize.Small, true, new List<Promotion>());
        _depositNoPromotions.AddAvailabilityPeriod(new DateRange.DateRange(Today, Today.AddDays(100)));
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists("BookingsReport.txt")) File.Delete("BookingsReport.txt");

        if (File.Exists("BookingsReport.csv")) File.Delete("BookingsReport.csv");
    }

    [TestMethod]
    public void TestCanCreateTxtBookingsReportFile()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new(1, _deposit!, Client, Today, Tomorrow, new Payment(50)),
            new(2, _depositNoPromotions!, Client, Tomorrow.AddDays(1), Tomorrow.AddDays(2), new Payment(70))
        };
        const string path = "BookingsReport.txt";
        var depositReport = "Deposit\t" +
                            $"{Today}-{Tomorrow}\t" +
                            "client@client.com\t" +
                            "35$\t" +
                            "Reserved\t" +
                            "Yes\n";
        var depositNoPromotionsReport = "Deposit\t" +
                                        $"{Tomorrow.AddDays(1)}-{Tomorrow.AddDays(2)}\t" +
                                        "client@client.com\t" +
                                        "70$\t" +
                                        "Reserved\t" +
                                        "No\n";
        var reportContent = depositReport + depositNoPromotionsReport;
        var reportGenerator = new TxtBookingReport();

        // Act
        reportGenerator.CreateReportFile(bookings, new PriceCalculator());

        // Assert
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual(reportContent, File.ReadAllText(path));
    }

    [TestMethod]
    public void TestCanCreateCsvBookingsReportFile()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new(1, _deposit!, Client, Today, Tomorrow, new Payment(35)),
            new(2, _depositNoPromotions!, Client, Tomorrow.AddDays(1), Tomorrow.AddDays(2), new Payment(70))
        };
        const string path = "BookingsReport.csv";
        var header = "Deposit,Client,StartDate,EndDate,Price,PaymentState,Promotions\n";
        var depositReport = "Deposit," +
                            "client@client.com," +
                            $"{Today:yyyy-MM-dd},{Tomorrow:yyyy-MM-dd}," +
                            "35$," +
                            "Reserved," +
                            "Yes\n";
        var depositNoPromotionsReport = "Deposit," +
                                        "client@client.com," +
                                        $"{Tomorrow.AddDays(1):yyyy-MM-dd},{Tomorrow.AddDays(2):yyyy-MM-dd}," +
                                        "70$," +
                                        "Reserved," +
                                        "No\n";
        var reportContent = header + depositReport + depositNoPromotionsReport;
        var reportGenerator = new CsvBookingReport();

        // Act
        reportGenerator.CreateReportFile(bookings, new PriceCalculator());

        // Assert
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual(reportContent, File.ReadAllText(path));
    }

    [TestMethod]
    public void TestCanCreateTxtBookingsReportFileWithRejectedBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new(1, _deposit!, Client, Today, Tomorrow, new Payment(50)),
            new(2, _depositNoPromotions!, Client, Tomorrow.AddDays(1), Tomorrow.AddDays(2), new Payment(70))
        };
        bookings[0].Reject("Message");
        bookings[1].Reject("Message");
        const string path = "BookingsReport.txt";
        var depositReport = "Deposit\t" +
                            $"{Today}-{Tomorrow}\t" +
                            "client@client.com\t" +
                            "35$\t" +
                            "Rejected\t" +
                            "Yes\n";
        var depositNoPromotionsReport = "Deposit\t" +
                                        $"{Tomorrow.AddDays(1)}-{Tomorrow.AddDays(2)}\t" +
                                        "client@client.com\t" +
                                        "70$\t" +
                                        "Rejected\t" +
                                        "No\n";
        var reportContent = depositReport + depositNoPromotionsReport;
        var reportGenerator = new TxtBookingReport();

        // Act
        reportGenerator.CreateReportFile(bookings, new PriceCalculator());

        // Assert
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual(reportContent, File.ReadAllText(path));
    }

    [TestMethod]
    public void TestCanCreateTxtBookingsReportFileWithApprovedBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new(1, _deposit!, Client, Today, Tomorrow, new Payment(50)),
            new(2, _depositNoPromotions!, Client, Tomorrow.AddDays(1), Tomorrow.AddDays(2), new Payment(70))
        };
        bookings[0].Approve();
        bookings[1].Approve();
        const string path = "BookingsReport.txt";
        var depositReport = "Deposit\t" +
                            $"{Today}-{Tomorrow}\t" +
                            "client@client.com\t" +
                            "35$\t" +
                            "Captured\t" +
                            "Yes\n";
        var depositNoPromotionsReport = "Deposit\t" +
                                        $"{Tomorrow.AddDays(1)}-{Tomorrow.AddDays(2)}\t" +
                                        "client@client.com\t" +
                                        "70$\t" +
                                        "Captured\t" +
                                        "No\n";
        var reportContent = depositReport + depositNoPromotionsReport;
        var reportGenerator = new TxtBookingReport();

        // Act
        reportGenerator.CreateReportFile(bookings, new PriceCalculator());

        // Assert
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual(reportContent, File.ReadAllText(path));
    }

    [TestMethod]
    public void TestCanCreateCsvBookingsReportFileWithRejectedBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new(1, _deposit!, Client, Today, Tomorrow, new Payment(35)),
            new(2, _depositNoPromotions!, Client, Tomorrow.AddDays(1), Tomorrow.AddDays(2), new Payment(70))
        };
        bookings[0].Reject("Message");
        bookings[1].Reject("Message");
        const string path = "BookingsReport.csv";
        var header = "Deposit,Client,StartDate,EndDate,Price,PaymentState,Promotions\n";
        var depositReport = "Deposit," +
                            "client@client.com," +
                            $"{Today:yyyy-MM-dd},{Tomorrow:yyyy-MM-dd}," +
                            "35$," +
                            "Rejected," +
                            "Yes\n";
        var depositNoPromotionsReport = "Deposit," +
                                        "client@client.com," +
                                        $"{Tomorrow.AddDays(1):yyyy-MM-dd},{Tomorrow.AddDays(2):yyyy-MM-dd}," +
                                        "70$," +
                                        "Rejected," +
                                        "No\n";
        var reportContent = header + depositReport + depositNoPromotionsReport;
        var reportGenerator = new CsvBookingReport();

        // Act
        reportGenerator.CreateReportFile(bookings, new PriceCalculator());

        // Assert
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual(reportContent, File.ReadAllText(path));
    }

    [TestMethod]
    public void TestCanCreateCsvBookingsReportFileWithApprovedBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new(1, _deposit!, Client, Today, Tomorrow, new Payment(35)),
            new(2, _depositNoPromotions!, Client, Tomorrow.AddDays(1), Tomorrow.AddDays(2), new Payment(70))
        };
        bookings[0].Approve();
        bookings[1].Approve();
        const string path = "BookingsReport.csv";
        var header = "Deposit,Client,StartDate,EndDate,Price,PaymentState,Promotions\n";
        var depositReport = "Deposit," +
                            "client@client.com," +
                            $"{Today:yyyy-MM-dd},{Tomorrow:yyyy-MM-dd}," +
                            "35$," +
                            "Captured," +
                            "Yes\n";
        var depositNoPromotionsReport = "Deposit," +
                                        "client@client.com," +
                                        $"{Tomorrow.AddDays(1):yyyy-MM-dd},{Tomorrow.AddDays(2):yyyy-MM-dd}," +
                                        "70$," +
                                        "Captured," +
                                        "No\n";
        var reportContent = header + depositReport + depositNoPromotionsReport;
        var reportGenerator = new CsvBookingReport();

        // Act
        reportGenerator.CreateReportFile(bookings, new PriceCalculator());

        // Assert
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual(reportContent, File.ReadAllText(path));
    }
}