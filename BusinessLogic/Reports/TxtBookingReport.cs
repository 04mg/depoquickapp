using Calculator;
using Domain;

namespace ReportGenerator;

public class TxtBookingReport : IBookingReport
{
    public string GenerateReportContent(Booking booking)
    {
        return $"{booking.Deposit.Name}\t" +
               $"{booking.Duration.StartDate}-{booking.Duration.EndDate}\t" +
               $"{booking.Client.Email}\t" +
               $"{new PriceCalculator().CalculatePrice(booking.Deposit, booking.Duration.StartDate, booking.Duration.EndDate)}$\t" +
               $"{GetPaymentState(booking)}\t" +
               $"{(booking.Deposit.Promotions.Count > 0 ? "Yes" : "No")}\n";
    }

    private static string GetPaymentState(Booking booking)
    {
        return booking.Payment == null ? "Rejected" : booking.Payment.Status.ToString();
    }
    
    public void CreateReportFile(IEnumerable<Booking> bookings)
    {
        const string path = $"BookingsReport.txt";
        var fileContent = bookings.Aggregate("", (current, booking) => current + GenerateReportContent(booking));
        File.WriteAllText(path, fileContent);
    }
}