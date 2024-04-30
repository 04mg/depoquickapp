namespace BusinessLogic;

public interface IPriceCalculator
{
    double CalculatePrice(Deposit deposit, Tuple<DateOnly, DateOnly> duration);
}