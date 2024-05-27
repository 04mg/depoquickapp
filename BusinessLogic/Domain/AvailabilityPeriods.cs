namespace BusinessLogic.Domain;

public class AvailabilityPeriods
{
    public List<DateRange> AvailablePeriods { get;} = new();

    public void AddAvailabilityPeriod(DateRange availabilityPeriod)
    {
        if (ExistsAnOverlappingPeriod(availabilityPeriod))
            MergePeriods(availabilityPeriod);
        else
            AvailablePeriods.Add(availabilityPeriod);
    }

    private void MergePeriods(DateRange availabilityPeriod)
    {
        var overlappingPeriod = AvailablePeriods.First(p => p.IsOverlapping(availabilityPeriod));
        overlappingPeriod.Merge(availabilityPeriod);
    }

    private bool ExistsAnOverlappingPeriod(DateRange availabilityPeriod)
    {
        return AvailablePeriods.Any(p => p.IsOverlapping(availabilityPeriod));
    }

    public void RemoveAvailabilityPeriod(DateRange dateRange)
    {
        var clonedAvailabilityPeriods = new List<DateRange>(AvailablePeriods);
        foreach (var period in clonedAvailabilityPeriods)
        {
            if (period.IsContained(dateRange))
            {
                RemoveContainedPeriod(dateRange);
            }
            else if (period.IsOverlapping(dateRange))
            {
                SubtractOverlappedPeriod(dateRange);
            }
        }
    }

    private void RemoveContainedPeriod(DateRange dateRange)
    {
        AvailablePeriods.Remove(AvailablePeriods.First(p => p.IsContained(dateRange)));
    }

    private void SubtractOverlappedPeriod(DateRange dateRange)
    {
        var overlappingPeriod = AvailablePeriods.First(p => p.IsOverlapping(dateRange));
        var result = overlappingPeriod.Subtract(dateRange);
        if (result != null)
        {
            AvailablePeriods.Add(result);
        }
    }
}