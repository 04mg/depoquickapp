namespace BusinessLogic;

public readonly struct PromotionModel
{
    public string Label { get; init; }
    public int Discount { get; init; }
    public DateOnly DateFrom { get; init; }
    public DateOnly DateTo { get; init; }
}