namespace BusinessLogic;

public readonly struct AddDepositDto
{
    public string Area { get; init; }
    public string Size { get; init; }
    public bool ClimateControl { get; init; }
    public List<int> PromotionList { get; init; }
}