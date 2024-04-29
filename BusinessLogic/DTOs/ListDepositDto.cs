namespace BusinessLogic.DTOs;

public readonly struct ListDepositDto
{
    public int Id { get; init; }
    public string Area { get; init; }
    public string Size { get; init; }
    public bool ClimateControl { get; init; }
    public List<int> PromotionList { get; init; }
}