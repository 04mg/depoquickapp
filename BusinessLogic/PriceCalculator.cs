namespace BusinessLogic;

public class PriceCalculator
{
    private const float SmallPricePerDay = 50;
    private const float MediumPricePerDay = 75;
    private const float LargePricePerDay = 100;
    private const float ClimateControlPrice = 20;
    private const int DurationDiscount1 = 5;
    private const int DurationDiscount2 = 10;
    private const int DurationDiscountThreshold1 = 7;
    private const int DurationDiscountThreshold2 = 14;

    public double CalculatePrice(Deposit deposit, Tuple<DateOnly, DateOnly> duration)
    {
        var discount = 0;
        float pricePerDay = deposit.Size switch
        {
            "Small" => SmallPricePerDay,
            "Medium" => MediumPricePerDay,
            "Large" => LargePricePerDay,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (deposit.ClimateControl)
        {
            pricePerDay += ClimateControlPrice;
        }

        switch (duration)
        {
            case var (dateFrom, dateTo) when dateTo.DayNumber - dateFrom.DayNumber < DurationDiscountThreshold1:
                discount += 0;
                break;
            case var (dateFrom, dateTo) when dateTo.DayNumber - dateFrom.DayNumber >= DurationDiscountThreshold1 &&
                                             dateTo.DayNumber - dateFrom.DayNumber <= DurationDiscountThreshold2:
                discount += DurationDiscount1;
                break;
            case var (dateFrom, dateTo) when dateTo.DayNumber - dateFrom.DayNumber > DurationDiscountThreshold2:
                discount += DurationDiscount2;
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
}