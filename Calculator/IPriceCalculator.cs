using Domain;

namespace Calculator;

public interface IPriceCalculator
{
    double CalculatePrice(Deposit deposit, DateOnly from, DateOnly to);
}