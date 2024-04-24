namespace BusinessLogic;

public class PromotionManager
{
    public List<Promotion> Promotions { get; private set; }

    public PromotionManager()
    {
        Promotions = new List<Promotion>();
    }

    private static void EnsureUserIsAdmin(Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
        {
            throw new UnauthorizedAccessException("Only administrators can manage promotions.");
        }
    }
    
    private void EnsurePromotionExists(int id)
    {
        if (Promotions.All(p => p.Id != id))
        {
            throw new ArgumentException("Promotion not found.");
        }
    }
    
    private Promotion GetPromotionById(int id)
    {
        return Promotions.First(p => p.Id == id);
    }

    public void Add(AddPromotionDto dto, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);

        Promotions.Add(new Promotion(NextPromotionId, dto.Label, dto.Discount, dto.DateFrom,
            dto.DateTo));
    }

    public void Delete(int id, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsurePromotionExists(id);

        var promotion = GetPromotionById(id);
        Promotions.Remove(promotion);
    }

    public void Modify(ModifyPromotionDto dto, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsurePromotionExists(dto.Id);
        
        var promotion = GetPromotionById(dto.Id);
        promotion.Label = dto.Label;
        promotion.Discount = dto.Discount;
        promotion.Validity = new Tuple<DateOnly, DateOnly>(dto.DateFrom, dto.DateTo);
    }
    
    public bool Exists(int id)
    {
        if (Promotions.All(p => p.Id == id))
        {
            return true;
        }

        return false;
    }

    private int NextPromotionId => Promotions.Count > 0 ? Promotions.Max(p => p.Id) + 1 : 1;
}