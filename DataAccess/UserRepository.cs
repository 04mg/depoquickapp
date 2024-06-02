using Domain;

namespace DataAccess;

public class UserRepository : IUserRepository
{
    private Dictionary<string, User> UsersByEmail { get; } = new();

    public void Add(User user)
    {
        UsersByEmail.Add(user.Email, user);
    }

    public User Get(string email)
    {
        return UsersByEmail[email];
    }

    public bool Exists(string email)
    {
        return UsersByEmail.ContainsKey(email);
    }
}