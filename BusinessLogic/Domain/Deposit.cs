using BusinessLogic.Enums;

namespace BusinessLogic.Domain;

public class Deposit
{
    private readonly string _area = "";
    private readonly string _size = "";
    private readonly string _name = "";

    public Deposit(string name, string area, string size, bool climateControl, List<Promotion> promotions)
    {
        Name = name;
        Area = area;
        Size = size;
        ClimateControl = climateControl;
        Promotions = promotions;
        AvailabilityPeriods = new List<DateRange>();
    }

    public bool ClimateControl { get; set; }
    public List<Promotion> Promotions { get; set; }

    public string Name
    {
        get => _name;
        private init
        {
            EnsureNameLengthIsValid(value);
            EnsureNameIsLettersOrSpaces(value);
            _name = value;
        }
    }
    public string Area
    {
        get => _area;
        private init
        {
            EnsureAreaIsValid(value);
            _area = value;
        }
    }

    public string Size
    {
        get => _size;
        private init
        {
            EnsureSizeIsValid(value);
            _size = value;
        }
    }

    public List<DateRange> AvailabilityPeriods { get; set; }

    private void EnsureNameLengthIsValid(string name)
    {
        if (name.Length == 0 || name.Length > 100)
            throw new ArgumentException("Name is invalid, it should be lesser or equal to 100 characters.");
    }
    private void EnsureNameIsLettersOrSpaces(string name)
    {
        if (!name.All(char.IsLetter) && !name.All(char.IsWhiteSpace))
            throw new ArgumentException("Name is invalid, it should only contain letters and whitespaces.");
    }
    
    private static void EnsureAreaIsValid(string area)
    {
        if (!Enum.TryParse<Area>(area, out _)) throw new ArgumentException("Area is invalid.");
    }

    private static void EnsureSizeIsValid(string size)
    {
        if (!Enum.TryParse<Size>(size, out _)) throw new ArgumentException("Size is invalid.");
    }

    public bool HasPromotion(int promotionId)
    {
        return Promotions.Any(p => p.Id == promotionId);
    }

    public int SumPromotions()
    {
        return Promotions.Sum(p => p.Discount);
    }

    public void AddAvailabilityPeriod(DateRange availabilityPeriod)
    {
        if (ExistsAnOverlappingPeriod(availabilityPeriod))
            MergePeriods(availabilityPeriod);
        else
            AvailabilityPeriods.Add(availabilityPeriod);
    }

    private void MergePeriods(DateRange availabilityPeriod)
    {
        var overlappingPeriod = AvailabilityPeriods.First(p => p.IsOverlapping(availabilityPeriod));
        overlappingPeriod.Merge(availabilityPeriod);
    }

    private bool ExistsAnOverlappingPeriod(DateRange availabilityPeriod)
    {
        return AvailabilityPeriods.Any(p => p.IsOverlapping(availabilityPeriod));
    }

    public void RemoveAvailabilityPeriod(DateRange dateRange)
    {
        var clonedAvailabilityPeriods = new List<DateRange>(AvailabilityPeriods);
        foreach (var period in clonedAvailabilityPeriods)
        {
            if(period.IsContained(dateRange))
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
        AvailabilityPeriods.Remove(AvailabilityPeriods.First(p => p.IsContained(dateRange)));
    }

    private void SubtractOverlappedPeriod(DateRange dateRange)
    {
        var overlappingPeriod = AvailabilityPeriods.First(p => p.IsOverlapping(dateRange));
        var result = overlappingPeriod.Subtract(dateRange);
        if (result != null)
        {
            AvailabilityPeriods.Add(result);
        }
    }
}