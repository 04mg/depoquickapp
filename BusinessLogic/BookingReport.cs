using BusinessLogic.Domain;

namespace BusinessLogic;

public class BookingReport
{
    private Booking Booking { get; }
    
    public BookingReport(Booking booking)
    {
        Booking = booking;
    }

    public string GenerateTxtReportContent()
    {
        return $"{Booking.Deposit.Name}\t" +
               $"{Booking.Duration.Item1}-{Booking.Duration.Item2}\t" +
               $"{Booking.Client.Email}\t" +
               $"{Booking.CalculatePrice()}$\t" +
               $"{(Booking.Deposit.Promotions.Count > 0 ? "Yes" : "No")}\n";
    }
    
    public string GenerateCsvReportContent()
    {
        const string tableHeader = "Deposit,Client,StartDate,EndDate,Price,Confirmed\n";

        return tableHeader +
               $"{Booking.Deposit.Name}," +
               $"{Booking.Client.Email}," +
               $"{Booking.Duration.Item1:yyyy-MM-dd}," +
               $"{Booking.Duration.Item2:yyyy-MM-dd}," +
               $"{Booking.CalculatePrice()}$," +
               $"{(Booking.Deposit.Promotions.Count > 0 ? "Yes" : "No")}\n";
    }

    public void CreateTxtReportFile()
    {
        var path = $"BookingReport_{Booking.Id}.txt";
        File.WriteAllText(path, GenerateTxtReportContent());
    }
    
    public void CreateCsvReportFile()
    {
        var path = $"BookingReport_{Booking.Id}.csv";
        File.WriteAllText(path, GenerateCsvReportContent());
    }
}