using BusinessLogic.Domain;

namespace BusinessLogic.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly List<Promotion> _promotions = new();
    private int NextPromotionId => _promotions.Count > 0 ? _promotions.Max(d => d.Id) + 1 : 1;

    public void Add(Promotion promotion)
    {
        promotion.Id = NextPromotionId;
        _promotions.Add(promotion);
    }

    public Promotion Get(int id)
    {
        return _promotions.First(promotion => promotion.Id == id);
    }

    public void Delete(int id)
    {
        _promotions.RemoveAll(promotion => promotion.Id == id);
    }

    public IEnumerable<Promotion> GetAll()
    {
        return _promotions;
    }

    public void Modify(int id, Promotion newPromotion)
    {
        var promotion = Get(id);
        promotion.Label = newPromotion.Label;
        promotion.Discount = newPromotion.Discount;
        promotion.Validity = newPromotion.Validity;
    }

    public bool Exists(int id)
    {
        return _promotions.Any(promotion => promotion.Id == id);
    }
}