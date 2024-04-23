namespace BusinessLogic;

public class Deposit
{
    private string _area;
    private string _small;
    private bool _climateControl;
    private List<AddPromotionDto> _promotionList;

    public string Area
    {
        get => _area;
        set
        {
            if(value != "A" && value != "B" && value != "C" && value != "D" && value != "E")
            {
                throw new ArgumentException("Invalid area.");
            }
            _area = value;
        }
    }
    public Deposit(string area, string small, bool climateControl, List<AddPromotionDto> promotionList)
    {
        Area = area;    
        _small = small;
        _climateControl = climateControl;
        _promotionList = promotionList;
    }
}

public enum Area
{
    A,
    B,
    C,
    D,
    E
}