namespace BusinessLogic.DTOs;

public struct AddDepositDto
{
    public string Area { init; get; }
    public string Size { init; get; }
    public bool ClimateControl { init; get; }
    public List<int> PromotionList { init; get; }
}