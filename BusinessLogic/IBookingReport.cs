using BusinessLogic.Domain;

namespace BusinessLogic;

public interface IBookingReport
{
    string GenerateReportContent(Booking booking);
    void CreateReportFile(IEnumerable<Booking> bookings);
}