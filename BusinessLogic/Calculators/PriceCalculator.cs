using Domain;
using Domain.Enums;

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

    public double CalculatePrice(Deposit deposit, DateOnly startDate, DateOnly endDate)
    {
        var pricePerDay = GetPricePerDay(deposit);
        var discount = GetTotalDiscount(deposit, startDate, endDate);
        var days = GetTotalDays(startDate, endDate);
        var basePrice = GetBasePrice(pricePerDay, days);
        var finalPrice = GetFinalPrice(basePrice, discount);
        return finalPrice;
    }

    private static double GetFinalPrice(double basePrice, int discount)
    {
        return basePrice - basePrice * discount / 100;
    }

    private static double GetBasePrice(double pricePerDay, int days)
    {
        return pricePerDay * days;
    }

    private static int GetTotalDays(DateOnly dateFrom, DateOnly dateTo)
    {
        return dateTo.DayNumber - dateFrom.DayNumber;
    }

    private static double GetPricePerDay(Deposit deposit)
    {
        double pricePerDay = deposit.Size switch
        {
            DepositSize.Small => SmallPricePerDay,
            DepositSize.Medium => MediumPricePerDay,
            DepositSize.Large => LargePricePerDay,
            _ => throw new ArgumentOutOfRangeException()
        };
        pricePerDay += GetClimateControlExtraPerDay(deposit);
        return pricePerDay;
    }

    private static double GetClimateControlExtraPerDay(Deposit deposit)
    {
        return deposit.ClimateControl ? ClimateControlPrice : 0;
    }

    private static int GetDurationDiscount(DateOnly dateFrom, DateOnly dateTo)
    {
        var days = GetTotalDays(dateFrom, dateTo);
        return days switch
        {
            < DurationThresholdForFirstDiscount => 0,
            <= DurationThresholdForSecondDiscount => FirstDiscount,
            _ => SecondDiscount
        };
    }

    private static int GetTotalDiscount(Deposit deposit, DateOnly dateFrom, DateOnly dateTo)
    {
        var durationDiscount = GetDurationDiscount(dateFrom, dateTo);
        var promotionsDiscount = deposit.SumPromotions();
        var totalDiscount = durationDiscount + promotionsDiscount;
        return totalDiscount > 100 ? 100 : totalDiscount;
    }
}