namespace BusinessLogic;

public readonly struct AddPromotionDto
{
    public string Label { init; get; }
    public int Discount { init; get; }
    public DateOnly DateFrom { init; get; }
    public DateOnly DateTo { init; get; }
}