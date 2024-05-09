using BusinessLogic.Domain;

namespace BusinessLogic.Calculators;

public interface IPriceCalculator
{
    double CalculatePrice(Deposit deposit, Tuple<DateOnly, DateOnly> duration);
}