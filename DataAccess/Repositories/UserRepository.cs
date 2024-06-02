using Domain;
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
        using var context = _contextFactory.CreateDbContext();
        context.Users.Add(user);
        context.SaveChanges();
    }

    public User Get(string email)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Users.Find(email) ?? throw new NullReferenceException();
    }

    public bool Exists(string email)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Users.Any(u => u.Email == email);
    }
}