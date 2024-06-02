using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly IDbContextFactory<Context> _contextFactory;

    public PromotionRepository(IDbContextFactory<Context> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public void Add(Promotion promotion)
    {
        using var context = _contextFactory.CreateDbContext();
        context.Promotions.Add(promotion);
        context.SaveChanges();
    }

    public Promotion Get(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Promotions.Find(id) ?? throw new NullReferenceException();
    }

    public void Delete(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var promotion = context.Promotions.Find(id);
        if (promotion != null) context.Promotions.Remove(promotion);
        context.SaveChanges();
    }

    public IEnumerable<Promotion> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Promotions.ToList();
    }

    public void Modify(int id, Promotion newPromotion)
    {
        using var context = _contextFactory.CreateDbContext();
        var promotion = Get(id);
        promotion.Label = newPromotion.Label;
        promotion.Discount = newPromotion.Discount;
        promotion.Validity = newPromotion.Validity;
        context.SaveChanges();
    }

    public bool Exists(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Promotions.Any(p => p.Id == id);
    }
}