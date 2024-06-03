using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class DepositRepository : IDepositRepository
{
    private readonly IDbContextFactory<Context> _contextFactory;

    public DepositRepository(IDbContextFactory<Context> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public void Add(Deposit deposit)
    {
        using var context = _contextFactory.CreateDbContext();
        context.AttachRange(deposit.Promotions);
        context.Deposits.Add(deposit);
        context.SaveChanges();
    }

    public Deposit Get(string name)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Deposits.First(d => string.Equals(d.Name.ToUpper(), name.ToUpper()));
    }

    public void Delete(string name)
    {
        using var context = _contextFactory.CreateDbContext();
        var deposit =
            context.Deposits.First(d => string.Equals(d.Name.ToUpper(), name.ToUpper()));
        context.Deposits.Remove(deposit);
        context.SaveChanges();
    }

    public IEnumerable<Deposit> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Deposits.ToList();
    }

    public bool Exists(string name)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Deposits.Any(d => string.Equals(d.Name.ToUpper(), name.ToUpper()));
    }
}