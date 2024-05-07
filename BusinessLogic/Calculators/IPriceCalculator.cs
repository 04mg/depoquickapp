using BusinessLogic.Domain;

namespace BusinessLogic.Interfaces;

public interface IPriceCalculator
{
    double CalculatePrice(Deposit deposit, Tuple<DateOnly, DateOnly> duration);
}