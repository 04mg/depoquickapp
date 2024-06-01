using BusinessLogic.Enums;

namespace BusinessLogic.Domain;

public class Payment : IPayment
{
    public double Amount { get; }
    private PaymentStatus Status { get; set; } = PaymentStatus.Reserved;

    public Payment(double amount)
    {
        Amount = amount;
    }

    public void Capture()
    {
        Status = PaymentStatus.Captured;
    }

    public bool IsCaptured()
    {
        return Status == PaymentStatus.Captured;
    }
}