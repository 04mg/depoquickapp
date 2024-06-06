namespace BusinessLogic.DTOs;

public struct PaymentDto
{
    public double Amount { get; init; }
    public bool Captured { get; init; }
}