using Domain.Exceptions;

namespace Domain;

public class AvailabilityPeriods
{
    public List<DateRange.DateRange> AvailablePeriods { get; set; }
    public List<DateRange.DateRange> UnavailablePeriods { get; set; }

    public AvailabilityPeriods()
    {
        AvailablePeriods = new List<DateRange.DateRange>();
        UnavailablePeriods = new List<DateRange.DateRange>();
    }

    public void AddAvailabilityPeriod(DateRange.DateRange newPeriod)
    {
        EnsurePeriodIsNotBooked(newPeriod);
        var clonedAvailablePeriods = new List<DateRange.DateRange>(AvailablePeriods);
        foreach (var period in clonedAvailablePeriods)
        {
            if (period.IsOverlapped(newPeriod))
            {
                MergeOverlappingPeriod(newPeriod);
            }
            else if (period.IsAdjacent(newPeriod))
            {
                MergeAdjacentPeriod(newPeriod);
            }
        }

        AvailablePeriods.Add(newPeriod);
    }

    private void EnsurePeriodIsNotBooked(DateRange.DateRange newPeriod)
    {
        if (!UnavailablePeriods.Any(newPeriod.IsOverlapped)) return;
        var overlappingPeriod = UnavailablePeriods.First(newPeriod.IsOverlapped);
        throw new DomainException(
            $"The availability period overlaps with an already booked period from {overlappingPeriod.StartDate} to {overlappingPeriod.EndDate}.");
    }

    private void MergeOverlappingPeriod(DateRange.DateRange newPeriod)
    {
        var overlappingPeriod = AvailablePeriods.First(p => p.IsOverlapped(newPeriod));
        newPeriod.Merge(overlappingPeriod);
        AvailablePeriods.Remove(overlappingPeriod);
    }

    private void MergeAdjacentPeriod(DateRange.DateRange newPeriod)
    {
        var overlappingPeriod = AvailablePeriods.First(p => p.IsAdjacent(newPeriod));
        newPeriod.Merge(overlappingPeriod);
        AvailablePeriods.Remove(overlappingPeriod);
    }

    public void RemoveAvailabilityPeriod(DateRange.DateRange dateRange)
    {
        var clonedAvailabilityPeriods = new List<DateRange.DateRange>(AvailablePeriods);
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

    private void RemoveContainedPeriod(DateRange.DateRange dateRange)
    {
        AvailablePeriods.Remove(AvailablePeriods.First(p => p.IsContained(dateRange)));
    }

    private void SubtractOverlappedPeriod(DateRange.DateRange dateRange)
    {
        var overlappingPeriod = AvailablePeriods.First(p => p.IsOverlapped(dateRange));
        var result = overlappingPeriod.Subtract(dateRange);
        if (result != null)
        {
            AvailablePeriods.Add(result);
        }
    }

    public bool IsAvailable(DateRange.DateRange dateRange)
    {
        return AvailablePeriods.Any(dateRange.IsContained) && !UnavailablePeriods.Any(dateRange.IsOverlapped);
    }

    public void MakePeriodAvailable(DateRange.DateRange dateRange)
    {
        var unavailablePeriod = UnavailablePeriods.First(dateRange.Equals);
        UnavailablePeriods.Remove(unavailablePeriod);
        AddAvailabilityPeriod(dateRange);
    }

    public void MakePeriodUnavailable(DateRange.DateRange dateRange)
    {
        RemoveAvailabilityPeriod(dateRange);
        UnavailablePeriods.Add(dateRange);
    }
}