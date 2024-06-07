using Domain;

namespace DataAccess.Repositories;

public interface IUserRepository : IRepository<string, User>
{
    bool Exists(string email);
    IEnumerable<User> GetAll();
}