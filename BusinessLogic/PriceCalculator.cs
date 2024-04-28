namespace BusinessLogic;

public class PriceCalculator
{
    public double CalculatePrice(Deposit deposit, Tuple<DateOnly, DateOnly> duration)
    {
        var discount = 0;
        float pricePerDay = deposit.Size switch
        {
            "Small" => 50,
            "Medium" => 75,
            "Large" => 100,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (deposit.ClimateControl)
        {
            pricePerDay += 20;
        }

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
}