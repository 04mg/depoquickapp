using BusinessLogic.Exceptions;

namespace BusinessLogic.Reports;

public abstract class BookingReportFactory
{
    public static IBookingReport Create(string type)
    {
        return type switch
        {
            "txt" => new TxtBookingReport(),
            "csv" => new CsvBookingReport(),
            _ => throw new BusinessLogicException("Invalid format. Supported formats: txt, csv.")
        };
    }
}