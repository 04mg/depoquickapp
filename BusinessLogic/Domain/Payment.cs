namespace BusinessLogic.Domain;

public class Payment
{
    public double Amount { get; }

    public Payment(double amount)
    {
        Amount = amount;
    }
}