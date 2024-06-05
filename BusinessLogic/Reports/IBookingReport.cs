using Domain;

namespace BusinessLogic.Reports;

public interface IBookingReport
{
    string GenerateReportContent(Booking booking);
    void CreateReportFile(IEnumerable<Booking> bookings);
}