using BusinessLogic.Enums;

namespace BusinessLogic.Domain;

public class Payment : IPayment
{
    public double Amount { get; }
    public PaymentStatus Status { get; private set; }

    public Payment(double amount)
    {
        Amount = amount;
    }
    
    public void Capture()
    {
        Status = PaymentStatus.Captured;
    }
}