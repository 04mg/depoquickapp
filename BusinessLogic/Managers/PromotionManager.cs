using BusinessLogic.Domain;
using BusinessLogic.DTOs;

namespace BusinessLogic.Managers;

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

    public void EnsurePromotionExists(int id)
    {
        if (Promotions.All(p => p.Id != id))
        {
            throw new ArgumentException("Promotion not found.");
        }
    }

    public Promotion GetPromotionById(int id)
    {
        return Promotions.First(p => p.Id == id);
    }

    public void Add(Promotion promotion, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        promotion.Id = NextPromotionId;
        Promotions.Add(promotion);
    }

    public void Delete(int id, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsurePromotionExists(id);
        var promotion = GetPromotionById(id);
        Promotions.Remove(promotion);
    }

    public void Modify(int id, Promotion newPromotion, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsurePromotionExists(id);

        var oldPromotion = GetPromotionById(id);
        oldPromotion.Label = newPromotion.Label;
        oldPromotion.Discount = newPromotion.Discount;
        oldPromotion.Validity = newPromotion.Validity;
    }

    public bool Exists(int id)
    {
        return Promotions.Any(p => p.Id == id);
    }

    private int NextPromotionId => Promotions.Count > 0 ? Promotions.Max(p => p.Id) + 1 : 1;
}