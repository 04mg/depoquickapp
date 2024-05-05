namespace BusinessLogic.DTOs;

public struct AddDepositDto
{
    public string Area { set; get; }
    public string Size { set; get; }
    public bool ClimateControl { set; get; }
    public List<int> PromotionList { set; get; }
}