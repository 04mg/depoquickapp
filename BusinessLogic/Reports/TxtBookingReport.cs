using Calculators.Interfaces;
using Domain;

namespace BusinessLogic.Reports;

public class TxtBookingReport : IBookingReport
{
    public void CreateReportFile(IEnumerable<Booking> bookings, IPriceCalculator priceCalculator)
    {
        const string path = "BookingsReport.txt";
        var fileContent = bookings.Aggregate("",
            (current, booking) => current + GenerateReportContent(booking, priceCalculator));
        File.WriteAllText(path, fileContent);
    }

    private static string GenerateReportContent(Booking booking, IPriceCalculator priceCalculator)
    {
        return $"{booking.GetDepositName()}\t" +
               $"{booking.Duration.StartDate}-{booking.Duration.EndDate}\t" +
               $"{booking.GetClientEmail()}\t" +
               $"{priceCalculator.CalculatePrice(booking.Deposit, booking.Duration.StartDate, booking.Duration.EndDate)}$\t" +
               $"{booking.GetPaymentStatus()}\t" +
               $"{(booking.GetPromotionsCount() > 0 ? "Yes" : "No")}\n";
    }
}