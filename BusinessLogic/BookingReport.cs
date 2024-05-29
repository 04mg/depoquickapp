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
        throw new NotImplementedException();
    }
}