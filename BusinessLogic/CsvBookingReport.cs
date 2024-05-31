using BusinessLogic.Calculators;
using BusinessLogic.Domain;

namespace BusinessLogic;

public class CsvBookingReport : IBookingReport
{
    public string GenerateReportContent(Booking booking)
    {
        return $"{booking.Deposit.Name}," +
               $"{booking.Client.Email}," +
               $"{booking.Duration.Item1:yyyy-MM-dd}," +
               $"{booking.Duration.Item2:yyyy-MM-dd}," +
               $"{new PriceCalculator().CalculatePrice(booking.Deposit, booking.Duration.Item1, booking.Duration.Item2)}$," +
               $"{(booking.Deposit.Promotions.Count > 0 ? "Yes" : "No")}\n";
    }
    
    public void CreateReportFile(IEnumerable<Booking> bookings)
    {
        const string path = $"BookingsReport.csv";
        const string header = "Deposit,Client,StartDate,EndDate,Price,Promotions\n";
        var fileContent = bookings.Aggregate(header, (current, booking) => current + GenerateReportContent(booking));
        File.WriteAllText(path, fileContent);
    }
}