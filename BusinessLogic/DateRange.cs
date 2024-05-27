namespace BusinessLogic;

public class DateRange
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public DateRange(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
        EnsureStartDateIsLesserThanEndDate(startDate, endDate);
    }

    private void EnsureStartDateIsLesserThanEndDate(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Date range is invalid.");
    }

    public bool IsOverlapping(DateRange other)
    {
        return !((StartDate > other.StartDate && StartDate > other.EndDate) ||
                (EndDate < other.StartDate && EndDate < other.EndDate));
    }

    public void Merge(DateRange other)
    {
        StartDate = StartDate < other.StartDate ? StartDate : other.StartDate;
        EndDate = EndDate > other.EndDate ? EndDate : other.EndDate;
    }

    public DateRange? Subtract(DateRange other)
    {
        if(!IsOverlapping(other))
            return null;
        if (StartDate == other.StartDate && EndDate == other.EndDate)
            return null;
        if (StartDate < other.StartDate && EndDate > other.EndDate)
        {
            var range1 = new DateRange(StartDate, other.StartDate.AddDays(-1));
            var range2 = new DateRange(other.EndDate.AddDays(1), EndDate);
            EndDate = range1.EndDate;
            return range2;
        }
        else if (StartDate < other.StartDate && EndDate <= other.EndDate)
        {
            EndDate = other.StartDate.AddDays(-1);
        }else if(StartDate >= other.StartDate && EndDate > other.EndDate)
        {
            StartDate = other.EndDate.AddDays(1);
        }

        return null;
    }

    public bool IsContained(DateRange dateRange)
    {
        return StartDate >= dateRange.StartDate && EndDate <= dateRange.EndDate;
    }
    
    public bool IsAdjacent(DateRange dateRange)
    {
        return StartDate == dateRange.EndDate.AddDays(1) || EndDate == dateRange.StartDate.AddDays(-1);
    }
}