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
        if (File.Exists("BookingsReport.txt"))
        {
            File.Delete("BookingsReport.txt");
        }
        
        if (File.Exists("BookingsReport.csv"))
        {
            File.Delete("BookingsReport.csv");
        }
    }

    [TestMethod]
    public void TestCanCreateTxtBookingsReportFile()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new(1, _deposit, Client, Today, Tomorrow),
            new(2, _depositNoPromotions, Client, Tomorrow.AddDays(1), Tomorrow.AddDays(2))
        };
        const string path = $"BookingsReport.txt";
        var reportContent = "Deposit\t" +
                            $"{Today}-{Tomorrow}\t" +
                            "client@client.com\t" +
                            "35$\t" +
                            "Yes\n"+ 
                            "Deposit\t" +
                            $"{Tomorrow.AddDays(1)}-{Tomorrow.AddDays(2)}\t" +
                            "client@client.com\t" +
                            "70$\t" +
                            "No\n";
        var reportGenerator = new BookingReportGenerator(new TxtBookingReport());
        
        // Act
        reportGenerator.GenerateReport(bookings);

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
            new(1, _deposit, Client, Today, Tomorrow),
            new(2, _depositNoPromotions, Client, Tomorrow.AddDays(1), Tomorrow.AddDays(2))
        };
        const string path = $"BookingsReport.csv";
        var reportContent = "Deposit,Client,StartDate,EndDate,Price,Promotions\n" +
                            "Deposit,client@client.com," +
                            $"{Today:yyyy-MM-dd},{Tomorrow:yyyy-MM-dd},35$,Yes\n" +
                            "Deposit,client@client.com," +
                            $"{Tomorrow.AddDays(1):yyyy-MM-dd},{Tomorrow.AddDays(2):yyyy-MM-dd},70$,No\n";
        var reportGenerator = new BookingReportGenerator(new CsvBookingReport());

        // Act
        reportGenerator.GenerateReport(bookings);

        // Assert
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual(reportContent, File.ReadAllText(path));
    }
}