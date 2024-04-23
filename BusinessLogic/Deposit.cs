namespace BusinessLogic;

public class Deposit
{
    private string _area;
    private string _size;
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

    public string Size
    {
        get => _size;
        set
        {
            if(value != "Small" && value != "Medium" && value != "Large")
            {
                throw new ArgumentException("Invalid size.");
            }
            _size = value;
        }
        
    }
    
    public Deposit(string area, string size, bool climateControl, List<AddPromotionDto> promotionList)
    {
        Area = area;    
        Size = size;
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