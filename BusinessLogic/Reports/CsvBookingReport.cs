using BusinessLogic.Calculators;
using Domain;

namespace BusinessLogic.Reports;

public class CsvBookingReport : IBookingReport
{
    public string GenerateReportContent(Booking booking)
    {
        return $"{booking.GetDepositName()}," +
               $"{booking.GetClientEmail()}," +
               $"{booking.Duration.StartDate:yyyy-MM-dd}," +
               $"{booking.Duration.EndDate:yyyy-MM-dd}," +
               $"{new PriceCalculator().CalculatePrice(booking.Deposit, booking.Duration.StartDate, booking.Duration.EndDate)}$," +
               $"{booking.GetPaymentStatus()}," +
               $"{(booking.GetPromotionsCount() > 0 ? "Yes" : "No")}\n";
    }

    public void CreateReportFile(IEnumerable<Booking> bookings)
    {
        const string path = $"BookingsReport.csv";
        const string header = "Deposit,Client,StartDate,EndDate,Price,PaymentState,Promotions\n";
        var fileContent = bookings.Aggregate(header, (current, booking) => current + GenerateReportContent(booking));
        File.WriteAllText(path, fileContent);
    }
}