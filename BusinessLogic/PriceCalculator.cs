namespace BusinessLogic;

public class PriceCalculator
{
    public double CalculatePrice(Deposit deposit, Tuple<DateOnly, DateOnly> duration)
    {
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

        return pricePerDay;
    }
}