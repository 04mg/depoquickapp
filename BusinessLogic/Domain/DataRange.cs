namespace BusinessLogic.Domain;

public class DateRange
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public DateRange(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}