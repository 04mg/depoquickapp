namespace BusinessLogic.DTOs;

public struct PromotionDto
{
    public int Id { init; get; }
    public string Label { init; get; }
    public int Discount { init; get; }
    public DateOnly DateFrom { init; get; }
    public DateOnly DateTo { init; get; }
}