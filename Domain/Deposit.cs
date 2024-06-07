using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain;

public class Deposit
{
    private readonly string _name = "";

    public Deposit()
    {
    }

    public Deposit(string name, DepositArea area, DepositSize size, bool climateControl, List<Promotion> promotions)
    {
        Name = name;
        Area = area;
        Size = size;
        ClimateControl = climateControl;
        Promotions = promotions;
        AvailabilityPeriods = new AvailabilityPeriods();
    }

    public AvailabilityPeriods AvailabilityPeriods { get; set; }
    public bool ClimateControl { get; set; }
    public List<Promotion> Promotions { get; set; }

    [Key]
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

    public DepositArea Area { get; set; }
    public DepositSize Size { get; set; }

    private static void EnsureNameLengthIsValid(string name)
    {
        if (name.Length is 0 or > 100)
            throw new ArgumentException("Name is invalid, it should be lesser or equal to 100 characters.");
    }

    private static void EnsureNameIsLettersOrSpaces(string name)
    {
        if (!name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            throw new ArgumentException("Name is invalid, it should only contain letters and whitespaces.");
    }

    public bool HasPromotion(int promotionId)
    {
        return Promotions.Any(p => p.Id == promotionId);
    }

    public int SumPromotions()
    {
        return Promotions.Sum(p => p.Discount);
    }

    public void AddAvailabilityPeriod(DateRange.DateRange dateRange)
    {
        AvailabilityPeriods.AddAvailabilityPeriod(dateRange);
    }

    public void RemoveAvailabilityPeriod(DateRange.DateRange dateRange)
    {
        AvailabilityPeriods.RemoveAvailabilityPeriod(dateRange);
    }

    public List<DateRange.DateRange> GetAvailablePeriods()
    {
        return AvailabilityPeriods.AvailablePeriods;
    }

    public bool IsAvailable(DateRange.DateRange dateRange)
    {
        return AvailabilityPeriods.IsAvailable(dateRange);
    }

    public void MakeAvailable(DateRange.DateRange dateRange)
    {
        AvailabilityPeriods.MakePeriodAvailable(dateRange);
    }

    public void MakeUnavailable(DateRange.DateRange dateRange)
    {
        AvailabilityPeriods.MakePeriodUnavailable(dateRange);
    }

    public int GetPromotionsCount()
    {
        return Promotions.Count;
    }
}