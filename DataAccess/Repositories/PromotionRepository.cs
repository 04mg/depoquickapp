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

    public void Update(Promotion promotion)
    {
        using var context = _contextFactory.CreateDbContext();
        context.Promotions.Update(promotion);
        context.SaveChanges();
    }

    public bool Exists(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Promotions.Any(p => p.Id == id);
    }
}