using Domain;

namespace BusinessLogic.Reports;

public interface IBookingReport
{
    void CreateReportFile(IEnumerable<Booking> bookings);
}