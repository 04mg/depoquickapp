using BusinessLogic.Enums;

namespace BusinessLogic.Domain;

public class Deposit
{
    private readonly string _area = "";
    private readonly string _size = "";

    public Deposit(string name, int id, string area, string size, bool climateControl, List<Promotion> promotions)
    {
        Name = name;
        Id = id;
        Area = area;
        Size = size;
        ClimateControl = climateControl;
        Promotions = promotions;
    }

    public string Name { get; set; }
    public bool ClimateControl { get; set; }
    public List<Promotion> Promotions { get; set; }
    public int Id { get; set; }

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