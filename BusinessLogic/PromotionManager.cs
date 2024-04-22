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

    public void Add(AddPromotionDto dto, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);

        Promotions.Add(new Promotion(NextPromotionId, dto.Label, dto.Discount, dto.DateFrom,
            dto.DateTo));
    }

    public void Delete(int id, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);

        foreach (var promotion in Promotions)
        {
            if (promotion.Id == id)
            {
                Promotions.Remove(promotion);
                return;
            }
        }
    }

    public void Modify(ModifyPromotionDto dto, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);

        foreach (var promotion in Promotions)
        {
            if (promotion.Id == dto.Id)
            {
                promotion.Label = dto.Label;
                promotion.Discount = dto.Discount;
                promotion.Validity = new Tuple<DateOnly, DateOnly>(dto.DateFrom, dto.DateTo);
                return;
            }
        }
    }

    private int NextPromotionId => Promotions.Count > 0 ? Promotions.Max(p => p.Id) + 1 : 1;
}