using Domain;

namespace BusinessLogic.Calculators;

public interface IPriceCalculator
{
    double CalculatePrice(Deposit deposit, DateOnly from, DateOnly to);
}