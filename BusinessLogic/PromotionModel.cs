namespace BusinessLogic;

public readonly struct PromotionModel
{
    public int Id { get; init; }
    public string Label { get; init; }
    public int Discount { get; init; }
    public DateOnly DateFrom { get; init; }
    public DateOnly DateTo { get; init; }
}