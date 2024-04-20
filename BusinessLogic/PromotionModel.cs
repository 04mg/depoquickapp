namespace BusinessLogic;

public readonly struct PromotionModel
{
    public string Label { get; init; }
    public int Discount { get; init; }
    public string DateFrom { get; init; }
    public string DateTo { get; init; }
}