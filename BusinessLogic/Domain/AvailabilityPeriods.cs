namespace BusinessLogic.Domain;

public class AvailabilityPeriods
{
    public List<DateRange> AvailablePeriods { get;} = new();
    public List<DateRange> UnavailablePeriods { get;} = new();

    public void AddAvailabilityPeriod(DateRange newPeriod)
    {
        EnsurePeriodIsNotBooked(newPeriod);
        var clonedAvailablePeriods = new List<DateRange>(AvailablePeriods);
        foreach (var period in clonedAvailablePeriods)
        {
            if (period.IsOverlapped(newPeriod))
            {
                MergeOverlappingPeriod(newPeriod);
            } else if (period.IsAdjacent(newPeriod))
            {
                MergeAdjacentPeriod(newPeriod);
            }
        }
        AvailablePeriods.Add(newPeriod);
    }

    private void EnsurePeriodIsNotBooked(DateRange newPeriod)
    {
        if (!UnavailablePeriods.Any(newPeriod.IsOverlapped)) return;
        var overlappingPeriod = UnavailablePeriods.FirstOrDefault(newPeriod.IsOverlapped);
        throw new ArgumentException($"The availability period overlaps with an already booked period from {overlappingPeriod.StartDate} to {overlappingPeriod.EndDate}.");
    }

    private void MergeOverlappingPeriod(DateRange newPeriod)
    {
        var overlappingPeriod = AvailablePeriods.First(p => p.IsOverlapped(newPeriod));
        newPeriod.Merge(overlappingPeriod);
        AvailablePeriods.Remove(overlappingPeriod);
    }
    
    private void MergeAdjacentPeriod(DateRange newPeriod)
    {
        var overlappingPeriod = AvailablePeriods.First(p => p.IsAdjacent(newPeriod));
        newPeriod.Merge(overlappingPeriod);
        AvailablePeriods.Remove(overlappingPeriod);
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
            else if (period.IsOverlapped(dateRange))
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
        var overlappingPeriod = AvailablePeriods.First(p => p.IsOverlapped(dateRange));
        var result = overlappingPeriod.Subtract(dateRange);
        if (result != null)
        {
            AvailablePeriods.Add(result);
        }
    }

    public bool IsAvailable(DateRange dateRange)
    {
        return AvailablePeriods.Any(dateRange.IsContained) && !UnavailablePeriods.Any(dateRange.IsOverlapped);
    }

    public void MakePeriodAvailable(DateRange dateRange)
    {
        var unavailablePeriod = UnavailablePeriods.First(dateRange.Equals);
        UnavailablePeriods.Remove(unavailablePeriod);
        AddAvailabilityPeriod(dateRange);
    }

    public void MakePeriodUnavailable(DateRange dateRange)
    {
        RemoveAvailabilityPeriod(dateRange);
        UnavailablePeriods.Add(dateRange);
    }
}