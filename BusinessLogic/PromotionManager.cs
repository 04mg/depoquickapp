namespace BusinessLogic;

public class PromotionManager
{
    public List<Promotion> Promotions { get; private set; }

    public PromotionManager()
    {
        Promotions = new List<Promotion>();
    }

    public void Add(PromotionModel promotionModel)
    {
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
        throw new NotImplementedException();
    }
}