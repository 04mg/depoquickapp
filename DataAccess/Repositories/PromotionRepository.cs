using DataAccess.Exceptions;
using Domain;
using Microsoft.Data.SqlClient;
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
        try
        {
            using var context = _contextFactory.CreateDbContext();
            context.Promotions.Add(promotion);
            context.SaveChanges();
        }
        catch (SqlException)
        {
            throw new DataAccessException("SQL Server error");
        }
        catch (DbUpdateException)
        {
            throw new DataAccessException("Changes could not be saved");
        }
    }

    public Promotion Get(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Promotions.Find(id) ?? throw new NullReferenceException();
    }

    public void Delete(int id)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var promotion = context.Promotions.Find(id);
            if (promotion != null) context.Promotions.Remove(promotion);
            context.SaveChanges();
        }
        catch (SqlException)
        {
            throw new DataAccessException("SQL Server error");
        }
        catch (DbUpdateException)
        {
            throw new DataAccessException("Changes could not be saved");
        }
    }

    public IEnumerable<Promotion> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Promotions.ToList();
    }

    public void Update(Promotion promotion)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            context.Promotions.Update(promotion);
            context.SaveChanges();
        }
        catch (SqlException)
        {
            throw new DataAccessException("SQL Server error");
        }
        catch (DbUpdateException)
        {
            throw new DataAccessException("Changes could not be saved");
        }
    }

    public bool Exists(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Promotions.Any(p => p.Id == id);
    }
}