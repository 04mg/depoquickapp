using Domain;

namespace ReportGenerator;

public interface IBookingReport
{
    string GenerateReportContent(Booking booking);
    void CreateReportFile(IEnumerable<Booking> bookings);
}