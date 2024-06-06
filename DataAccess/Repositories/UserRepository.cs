using DataAccess.Exceptions;
using Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private IDbContextFactory<Context> _contextFactory;

    public UserRepository(IDbContextFactory<Context> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public void Add(User user)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            context.Users.Add(user);
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

    public User Get(string email)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Users.Find(email) ?? throw new NullReferenceException();
        }
        catch (SqlException)
        {
            throw new DataAccessException("SQL Server error");
        }
    }

    public bool Exists(string email)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Users.Any(u => u.Email == email);
        }
        catch (SqlException)
        {
            throw new DataAccessException("SQL Server error");
        }
    }
}