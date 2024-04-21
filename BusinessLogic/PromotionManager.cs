namespace BusinessLogic;

public class PromotionManager
{
    public List<Promotion> Promotions { get; private set; }

    public PromotionManager()
    {
        Promotions = new List<Promotion>();
    }

    public void Add(PromotionModel promotionModel, Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
        {
            throw new UnauthorizedAccessException("Only administrators can add promotions.");
        }
        Promotions.Add(new Promotion(promotionModel));
    }

    public void Delete(PromotionModel model)
    {
        foreach (var promotion in Promotions)
        {
            var dateFrom = DateOnly.FromDateTime(DateTime.Parse(model.DateFrom));
            var dateTo = DateOnly.FromDateTime(DateTime.Parse(model.DateTo));
            if (promotion.Label == model.Label && promotion.Discount == model.Discount &&
                promotion.Validity.Item1 == dateFrom && promotion.Validity.Item2 == dateTo)
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
            var dateFrom = DateOnly.FromDateTime(DateTime.Parse(model.DateFrom));
            var dateTo = DateOnly.FromDateTime(DateTime.Parse(model.DateTo));
            if (promotion.Label == model.Label && promotion.Discount == model.Discount &&
                promotion.Validity.Item1 == dateFrom && promotion.Validity.Item2 == dateTo)
            {
                promotion.Label = newModel.Label;
                promotion.Discount = newModel.Discount;
                promotion.Validity = new Tuple<DateOnly, DateOnly>(
                    DateOnly.FromDateTime(DateTime.Parse(newModel.DateFrom)),
                    DateOnly.FromDateTime(DateTime.Parse(newModel.DateTo)));
                return;
            }
        }
    }
}