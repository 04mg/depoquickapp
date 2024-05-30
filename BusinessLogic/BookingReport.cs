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
        const string tableHeader = "Deposit,Client,StartDate,EndDate,Price,Confirmed\n";

        return tableHeader +
               $"{booking.Deposit.Name}," +
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
}