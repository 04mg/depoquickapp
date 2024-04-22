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

        Promotions.Add(new Promotion(model.Label, model.Discount, model.DateFrom,
            model.DateTo));
    }

    public void Delete(PromotionModel model)
    {
        foreach (var promotion in Promotions)
        {
            if (promotion.Label == model.Label && promotion.Discount == model.Discount &&
                promotion.Validity.Item1 == model.DateFrom && promotion.Validity.Item2 == model.DateTo)
            {
                Promotions.Remove(promotion);
                return;
            }
        }
    }

    public void Modify(PromotionModel model, PromotionModel newModel)
    {
        foreach (var promotion in Promotions)
        {
            if (promotion.Label == model.Label && promotion.Discount == model.Discount &&
                promotion.Validity.Item1 == model.DateFrom && promotion.Validity.Item2 == model.DateTo)
            {
                promotion.Label = newModel.Label;
                promotion.Discount = newModel.Discount;
                promotion.Validity = new Tuple<DateOnly, DateOnly>(newModel.DateFrom, newModel.DateTo);
                return;
            }
        }
    }
}