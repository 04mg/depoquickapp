namespace BusinessLogic.DTOs;

public struct PriceDto
{
    public DepositDto Deposit { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
}