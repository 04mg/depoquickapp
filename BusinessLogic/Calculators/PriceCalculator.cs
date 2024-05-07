using BusinessLogic.Domain;

namespace BusinessLogic.Calculators;

public class PriceCalculator : IPriceCalculator
{
    private const double SmallPricePerDay = 50;
    private const double MediumPricePerDay = 75;
    private const double LargePricePerDay = 100;
    private const double ClimateControlPrice = 20;
    private const int FirstDiscount = 5;
    private const int SecondDiscount = 10;
    private const int DurationThresholdForFirstDiscount = 7;
    private const int DurationThresholdForSecondDiscount = 14;

    public double CalculatePrice(Deposit deposit, Tuple<DateOnly, DateOnly> duration)
    {
        var pricePerDay = GetPricePerDay(deposit.Size, deposit.ClimateControl);
        var discount = GetTotalDiscount(duration, deposit.Promotions);
        var days = duration.Item2.DayNumber - duration.Item1.DayNumber;
        var basePrice = pricePerDay * days;
        var finalPrice = basePrice - (basePrice * discount / 100);
        return finalPrice;
    }

    private static double GetPricePerDay(string size, bool climateControl)
    {
        var pricePerDay = size switch
        {
            "Small" => SmallPricePerDay,
            "Medium" => MediumPricePerDay,
            "Large" => LargePricePerDay,
            _ => throw new ArgumentOutOfRangeException(nameof(size))
        };
        pricePerDay += GetClimateControlExtraPerDay(climateControl);
        return pricePerDay;
    }

    private static double GetClimateControlExtraPerDay(bool climateControl)
    {
        return climateControl ? ClimateControlPrice : 0;
    }

    private static int GetDurationDiscount(Tuple<DateOnly, DateOnly> duration)
    {
        var days = duration.Item2.DayNumber - duration.Item1.DayNumber;
        return days switch
        {
            < DurationThresholdForFirstDiscount => 0,
            <= DurationThresholdForSecondDiscount => FirstDiscount,
            _ => SecondDiscount
        };
    }

    private static int GetPromotionsDiscount(IEnumerable<Promotion> promotions)
    {
        return promotions.Sum(promotion => promotion.Discount);
    }
    
    private static int GetTotalDiscount(Tuple<DateOnly, DateOnly> duration, IEnumerable<Promotion> promotions)
    {
        var durationDiscount = GetDurationDiscount(duration);
        var promotionsDiscount = GetPromotionsDiscount(promotions);
        var totalDiscount = durationDiscount + promotionsDiscount;
        return totalDiscount > 100 ? 100 : totalDiscount;
    }
}