using Domain;

namespace BusinessLogic.Reports;

public class BookingReportGenerator
{
    public BookingReportGenerator(IBookingReport bookingReport)
    {
        BookingReport = bookingReport;
    }
    
    public IBookingReport BookingReport { get; set; }
    
    public void GenerateReport(IEnumerable<Booking> bookings)
    {
        BookingReport.CreateReportFile(bookings);
    }
}