using BusinessLogic.Enums;

namespace BusinessLogic.Domain;

public class Deposit
{
    private readonly string _area = "";
    private readonly string _size = "";
    private readonly string _name = "";

    public Deposit(string name, string area, string size, bool climateControl, List<Promotion> promotions)
    {
        Name = name;
        Area = area;
        Size = size;
        ClimateControl = climateControl;
        Promotions = promotions;
    }

    public bool ClimateControl { get; set; }
    public List<Promotion> Promotions { get; set; }

    public string Name
    {
        get => _name;
        private init
        {
            EnsureNameIsLettersOrSpaces(value);
            _name = value;
        }
    }
    public string Area
    {
        get => _area;
        private init
        {
            EnsureAreaIsValid(value);
            _area = value;
        }
    }

    public string Size
    {
        get => _size;
        private init
        {
            EnsureSizeIsValid(value);
            _size = value;
        }
    }
    
    private void EnsureNameIsLettersOrSpaces(string name)
    {
        if (!name.All(char.IsLetter) && !name.All(char.IsWhiteSpace))
            throw new ArgumentException("Name is invalid, it should only contain letters and whitespaces.");
    }
    
    private static void EnsureAreaIsValid(string area)
    {
        if (!Enum.TryParse<Area>(area, out _)) throw new ArgumentException("Area is invalid.");
    }

    private static void EnsureSizeIsValid(string size)
    {
        if (!Enum.TryParse<Size>(size, out _)) throw new ArgumentException("Size is invalid.");
    }

    public bool HasPromotion(int promotionId)
    {
        return Promotions.Any(p => p.Id == promotionId);
    }

    public int SumPromotions()
    {
        return Promotions.Sum(p => p.Discount);
    }
}