namespace BusinessLogic;

public struct ModifyPromotionDto
{
    public int Id { get; set; }
    public string Label { get; set; }
    public int Discount { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
}