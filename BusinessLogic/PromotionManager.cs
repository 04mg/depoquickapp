namespace BusinessLogic;

public class PromotionManager
{
    public List<Promotion> Promotions { get; private set; }

    public PromotionManager()
    {
        Promotions = new List<Promotion>();
    }

    public void Add(PromotionModel model, Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
        {
            throw new UnauthorizedAccessException("Only administrators can add promotions.");
        }

        Promotions.Add(new Promotion( NextPromotionId, model.Label, model.Discount, model.DateFrom,
            model.DateTo));
    }

    public void Delete(int id)
    {
        foreach (var promotion in Promotions)
        {
            if (promotion.Id == id)
            {
                Promotions.Remove(promotion);
                return;
            }
        }
    }

    public void Modify(int id, PromotionModel newModel)
    {
        foreach (var promotion in Promotions)
        {
            if (promotion.Id == id)
            {
                promotion.Label = newModel.Label;
                promotion.Discount = newModel.Discount;
                promotion.Validity = new Tuple<DateOnly, DateOnly>(newModel.DateFrom, newModel.DateTo);
                return;
            }
        }
    }
    
    private int NextPromotionId => Promotions.Count > 0 ? Promotions.Max(p => p.Id) + 1 : 1;

}