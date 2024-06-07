using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain;

[Table("Payments")]
public class Payment
{
    public int Id { get; set; }
    public double Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Reserved;

    public Payment()
    {
    }

    public Payment(double amount)
    {
        Amount = amount;
    }

    public void Capture()
    {
        Status = PaymentStatus.Captured;
    }
}