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
    
    public bool IsOverlapping(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }
    
    public void Merge(DateRange other)
    {
        StartDate = StartDate < other.StartDate ? StartDate : other.StartDate;
        EndDate = EndDate > other.EndDate ? EndDate : other.EndDate;
    }
}