namespace BusinessLogic;

public class Deposit
{
    private string _area;
    private string _small;
    private bool _climateControl;
    private List<AddPromotionDto> _promotionList;
    public Deposit(string area, string small, bool climateControl, List<AddPromotionDto> promotionList)
    {
        _area = area;
        _small = small;
        _climateControl = climateControl;
        _promotionList = promotionList;
    }
}