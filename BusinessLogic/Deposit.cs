namespace BusinessLogic;

public class Deposit
{
    private string _area;
    private string _size;
    private bool _climateControl;
    private List<int> _promotionList;

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
    
    public Deposit(string area, string size, bool climateControl, List<int> promotionList, PromotionManager promotionManager)
    {
        Area = area;    
        Size = size;
        _climateControl = climateControl;
        foreach (var id in promotionList)
        {
            if(promotionManager.Promotions.All(p => p.Id != id))
            {
                throw new ArgumentException("Promotion with id " + id + " does not exist.");
            }
        }
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