using Domain;

namespace DataAccess;

public interface IUserRepository : IRepository<string, User>
{
    public bool Exists(string email);
}