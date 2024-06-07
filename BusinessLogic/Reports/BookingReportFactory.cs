namespace BusinessLogic.Reports;

public abstract class BookingReportFactory
{
    public static IBookingReport Create(string type)
    {
        return type switch
        {
            "txt" => new TxtBookingReport(),
            "csv" => new CsvBookingReport(),
            _ => throw new ArgumentException("Invalid format. Supported formats: txt, csv.")
        };
    }
}