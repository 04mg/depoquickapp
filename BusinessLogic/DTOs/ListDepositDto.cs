namespace BusinessLogic.DTOs;

public struct ListDepositDto
{
    public int Id { get; set; }
    public string Area { get; set; }
    public string Size { get; set; }
    public bool ClimateControl { get; set; }
    public List<int> PromotionList { get; set; }
}