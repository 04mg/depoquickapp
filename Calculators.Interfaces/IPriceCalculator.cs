using Domain;

namespace Calculators;

public interface IPriceCalculator
{
    double CalculatePrice(Deposit deposit, DateOnly startDate, DateOnly endDate);
}