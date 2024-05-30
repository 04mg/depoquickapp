using BusinessLogic.Domain;

namespace BusinessLogic;

public class BookingReport
{
    public static string GenerateTxtReportContent(Booking booking)
    {
        return $"{booking.Deposit.Name}\t" +
               $"{booking.Duration.Item1}-{booking.Duration.Item2}\t" +
               $"{booking.Client.Email}\t" +
               $"{booking.CalculatePrice()}$\t" +
               $"{(booking.Deposit.Promotions.Count > 0 ? "Yes" : "No")}\n";
    }

    public static string GenerateCsvReportContent(Booking booking)
    {
        return $"{booking.Deposit.Name}," +
               $"{booking.Client.Email}," +
               $"{booking.Duration.Item1:yyyy-MM-dd}," +
               $"{booking.Duration.Item2:yyyy-MM-dd}," +
               $"{booking.CalculatePrice()}$," +
               $"{(booking.Deposit.Promotions.Count > 0 ? "Yes" : "No")}\n";
    }

    public static void CreateTxtReportFile(Booking booking)
    {
        var path = $"BookingReport_{booking.Id}.txt";
        File.WriteAllText(path, GenerateTxtReportContent(booking));
    }

    public static void CreateCsvReportFile(Booking booking)
    {
        var path = $"BookingReport_{booking.Id}.csv";
        File.WriteAllText(path, GenerateCsvReportContent(booking));
    }
    
    public static void CreateBookingsReportFileTxt(IEnumerable<Booking> bookings)
    {
        const string path = $"BookingsReport.txt";
        var fileContent = bookings.Aggregate("", (current, booking) => current + GenerateTxtReportContent(booking));
        File.WriteAllText(path, fileContent);
    }
    
    public static void CreateBookingsReportFileCsv(IEnumerable<Booking> bookings)
    {
        const string path = $"BookingsReport.csv";
        const string header = "Deposit,Client,StartDate,EndDate,Price,Promotions\n";
        var fileContent = bookings.Aggregate(header, (current, booking) => current + GenerateCsvReportContent(booking));
        File.WriteAllText(path, fileContent);
    }
}