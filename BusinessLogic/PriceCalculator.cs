namespace BusinessLogic;

public class PriceCalculator
{
    private const double SmallPricePerDay = 50;
    private const double MediumPricePerDay = 75;
    private const double LargePricePerDay = 100;
    private const double ClimateControlPrice = 20;
    private const int DurationDiscount1 = 5;
    private const int DurationDiscount2 = 10;
    private const int DurationDiscountThreshold1 = 7;
    private const int DurationDiscountThreshold2 = 14;

    public double CalculatePrice(Deposit deposit, Tuple<DateOnly, DateOnly> duration)
    {
        var discount = 0;
        var pricePerDay = GetPricePerDay(deposit.Size, deposit.ClimateControl);

        switch (duration)
        {
            case var (dateFrom, dateTo) when dateTo.DayNumber - dateFrom.DayNumber < 7:
                discount += 0;
                break;
            case var (dateFrom, dateTo) when dateTo.DayNumber - dateFrom.DayNumber >= 7 &&
                                             dateTo.DayNumber - dateFrom.DayNumber <= 14:
                discount += 5;
                break;
            case var (dateFrom, dateTo) when dateTo.DayNumber - dateFrom.DayNumber > 14:
                discount += 10;
                break;
        }

        if (deposit.Promotions.Count > 0)
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
}