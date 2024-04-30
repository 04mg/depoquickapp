namespace BusinessLogic;

public struct AddDepositDto
{
    public string Area { get; set; }
    public string Size { get; set; }
    public bool ClimateControl { get; set; }
    public List<int> PromotionList { get; set; }
}