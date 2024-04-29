namespace BusinessLogic;

public class PriceCalculator
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
        var discount = GetDurationDiscount(duration);
        
        if(deposit.Promotions.Count > 0)
        {
            discount += deposit.Promotions.Sum(promotion => promotion.Discount);
        }

        if (discount > 100)
        {
            discount = 100;
        }

        var basePrice = pricePerDay * (duration.Item2.DayNumber - duration.Item1.DayNumber);
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
        var durationInDays = duration.Item2.DayNumber - duration.Item1.DayNumber;
        return durationInDays switch
        {
            < DurationThresholdForFirstDiscount => 0,
            <= DurationThresholdForSecondDiscount => FirstDiscount,
            _ => SecondDiscount
        };
    }
}