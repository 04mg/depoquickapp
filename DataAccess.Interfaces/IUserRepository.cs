using Domain;

namespace DataAccess.Interfaces;

public interface IUserRepository
{
    void Add(User user);
    User Get(string email);
    bool Exists(string email);
    IEnumerable<User> GetAll();
}