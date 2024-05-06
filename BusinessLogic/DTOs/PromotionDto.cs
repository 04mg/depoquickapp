namespace BusinessLogic.DTOs;

public struct PromotionDto
{
    public int Id { init; get; }
    public string Label { set; get; }
    public int Discount { set; get; }
    public DateOnly DateFrom { set; get; }
    public DateOnly DateTo { set; get; }
}