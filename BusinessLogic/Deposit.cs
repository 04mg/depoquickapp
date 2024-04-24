namespace BusinessLogic;

public class Deposit
{
    private string _area = "";
    private string _size = "";
    public bool ClimateControl { get; set; }
    public List<int> PromotionList { get; set; }
    public int Id { get; }
    
    public string Area
    {
        get => _area;
        set
        {
            EnsureAreaIsValid(value);
            _area = value;
        }
    }

    public string Size
    {
        get => _size;
        set
        {
            EnsureSizeIsValid(value);
            _size = value;
        }
        
    }
    
    public Deposit(int id, string area, string size, bool climateControl, List<int> promotionList, PromotionManager promotionManager)
    {
        Id = id;
        Area = area;    
        Size = size;
        ClimateControl = climateControl;
        EnsurePromotionExists(promotionList, promotionManager);
        PromotionList = promotionList;
    }

    private static void EnsurePromotionExists(List<int> promotionList, PromotionManager promotionManager)
    {
        foreach (var id in promotionList)
        {
            EnsurePromotionExistsForId(promotionManager, id);
        }
    }

    private static void EnsurePromotionExistsForId(PromotionManager promotionManager, int id)
    {
        if(!promotionManager.Exists(id))
        {
            throw new ArgumentException("Promotion with id " + id + " does not exist.");
        }
    }

    private static void EnsureAreaIsValid(string area)
    {
        if (!Enum.TryParse<Area>(area, out _))
        {
            throw new ArgumentException("Area is invalid.");
        }
    }
    
    private static void EnsureSizeIsValid(string size)
    {
        if (!Enum.TryParse<Size>(size, out _))
        {
            throw new ArgumentException("Size is invalid.");
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

public enum Size
{
    Small,
    Medium,
    Large
}