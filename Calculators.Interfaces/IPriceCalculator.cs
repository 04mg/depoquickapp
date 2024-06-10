using Domain;

namespace Calculators.Interfaces;

public interface IPriceCalculator
{
    double CalculatePrice(Deposit deposit, DateOnly startDate, DateOnly endDate);
}