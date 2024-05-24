using BusinessLogic.Domain;

namespace BusinessLogic.Repositories;

public interface IUserRepository : IRepository<string, User>
{
    public bool Exists(string email);
}