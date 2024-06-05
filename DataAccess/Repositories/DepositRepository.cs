using DataAccess.Exceptions;
using Domain;
using Microsoft.Data.SqlClient;
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
        try
        {
            using var context = _contextFactory.CreateDbContext();
            context.AttachRange(deposit.Promotions);
            context.AttachRange(context.Deposits.Include(d => d.AvailabilityPeriods));
            context.Deposits.Add(deposit);
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

    public Deposit Get(string name)
    {
            using var context = _contextFactory.CreateDbContext();
            return context.Deposits.Include(d => d.Promotions).Include(d => d.AvailabilityPeriods)
                .First(d => string.Equals(d.Name.ToUpper(), name.ToUpper()));
    }

    public void Delete(string name)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var deposit =
                context.Deposits.First(d => string.Equals(d.Name.ToUpper(), name.ToUpper()));
            context.Deposits.Remove(deposit);
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

    public IEnumerable<Deposit> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Deposits.Include(d => d.Promotions).Include(d => d.AvailabilityPeriods).ToList();
    }

    public bool Exists(string name)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Deposits.Any(d => string.Equals(d.Name.ToUpper(), name.ToUpper()));
    }

    public void Update(Deposit deposit)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var existingDeposit = context.Deposits.Include(d => d.AvailabilityPeriods)
                .First(d => string.Equals(d.Name.ToUpper(), deposit.Name.ToUpper()));
            context.Entry(existingDeposit).CurrentValues.SetValues(deposit);
            context.Entry(existingDeposit).Reference(d => d.AvailabilityPeriods).CurrentValue = deposit.AvailabilityPeriods;
            context.Update(existingDeposit);
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
}