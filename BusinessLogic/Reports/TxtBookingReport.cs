using BusinessLogic.Calculators;
using Domain;

namespace BusinessLogic.Reports;

public class TxtBookingReport : IBookingReport
{
    public string GenerateReportContent(Booking booking)
    {
        return $"{booking.GetDepositName()}\t" +
               $"{booking.Duration.StartDate}-{booking.Duration.EndDate}\t" +
               $"{booking.GetClientEmail()}\t" +
               $"{new PriceCalculator().CalculatePrice(booking.Deposit, booking.Duration.StartDate, booking.Duration.EndDate)}$\t" +
               $"{booking.GetPaymentStatus()}\t" +
               $"{(booking.GetPromotionsCount() > 0 ? "Yes" : "No")}\n";
    }
    
    public void CreateReportFile(IEnumerable<Booking> bookings)
    {
        const string path = $"BookingsReport.txt";
        var fileContent = bookings.Aggregate("", (current, booking) => current + GenerateReportContent(booking));
        File.WriteAllText(path, fileContent);
    }
}