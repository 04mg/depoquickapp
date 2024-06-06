using Domain;

namespace BusinessLogic.Reports;

public class BookingReportGenerator
{
    public BookingReportGenerator(IBookingReport bookingReport)
    {
        BookingReport = bookingReport;
    }

    private IBookingReport BookingReport { get;}
    
    public static BookingReportGenerator CreateReportGenerator(string type)
    {
        return type switch
        {
            "txt" => new BookingReportGenerator(new TxtBookingReport()),
            "csv" => new BookingReportGenerator(new CsvBookingReport()),
            _ => throw new ArgumentException("Invalid format. Supported formats: txt, csv.")
        };
    }
    public void GenerateReport(IEnumerable<Booking> bookings)
    {
        BookingReport.CreateReportFile(bookings);
    }
}