using Calculator;
using Domain;

namespace ReportGenerator;

public class CsvBookingReport : IBookingReport
{
    public string GenerateReportContent(Booking booking)
    {
        return $"{booking.Deposit.Name}," +
               $"{booking.Client.Email}," +
               $"{booking.Duration.StartDate:yyyy-MM-dd}," +
               $"{booking.Duration.EndDate:yyyy-MM-dd}," +
               $"{new PriceCalculator().CalculatePrice(booking.Deposit, booking.Duration.StartDate, booking.Duration.EndDate)}$," +
               $"{GetPaymentState(booking)}," +
               $"{(booking.Deposit.Promotions.Count > 0 ? "Yes" : "No")}\n";
    }

    private static string GetPaymentState(Booking booking)
    {
        return booking.Payment == null ? "Rejected" : booking.Payment.Status.ToString();
    }

    public void CreateReportFile(IEnumerable<Booking> bookings)
    {
        const string path = $"BookingsReport.csv";
        const string header = "Deposit,Client,StartDate,EndDate,Price,PaymentState,Promotions\n";
        var fileContent = bookings.Aggregate(header, (current, booking) => current + GenerateReportContent(booking));
        File.WriteAllText(path, fileContent);
    }
}