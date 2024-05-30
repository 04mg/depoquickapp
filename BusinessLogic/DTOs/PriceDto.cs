namespace BusinessLogic.DTOs;

public struct PriceDto
{
    public string DepositName { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
}