using Domain;

namespace BusinessLogic.Calculators;

public interface IPriceCalculator
{
    double CalculatePrice(Deposit deposit, DateOnly startDate, DateOnly endDate);
}