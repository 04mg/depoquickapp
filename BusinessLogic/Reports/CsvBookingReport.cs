using Calculators.Interfaces;
using Domain;

namespace BusinessLogic.Reports;

public class CsvBookingReport : IBookingReport
{
    public void CreateReportFile(IEnumerable<Booking> bookings, IPriceCalculator priceCalculator)
    {
        const string path = "BookingsReport.csv";
        const string header = "Deposit,Client,StartDate,EndDate,Price,PaymentState,Promotions\n";
        var fileContent = bookings.Aggregate(header,
            (current, booking) => current + GenerateReportContent(booking, priceCalculator));
        File.WriteAllText(path, fileContent);
    }

    private static string GenerateReportContent(Booking booking, IPriceCalculator priceCalculator)
    {
        return $"{booking.GetDepositName()}," +
               $"{booking.GetClientEmail()}," +
               $"{booking.Duration.StartDate:yyyy-MM-dd}," +
               $"{booking.Duration.EndDate:yyyy-MM-dd}," +
               $"{priceCalculator.CalculatePrice(booking.Deposit, booking.Duration.StartDate, booking.Duration.EndDate)}$," +
               $"{booking.GetPaymentStatus()}," +
               $"{(booking.GetPromotionsCount() > 0 ? "Yes" : "No")}\n";
    }
}