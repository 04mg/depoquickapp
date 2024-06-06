using Domain;

namespace DataAccess.Repositories;

public interface IUserRepository : IRepository<string, User>
{
    public bool Exists(string email);
}