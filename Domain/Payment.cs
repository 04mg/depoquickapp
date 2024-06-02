using Domain.Enums;

namespace Domain;

public class Payment : IPayment
{
    private double Amount { get; }
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

    public double GetAmount()
    {
        return Amount;
    }
}