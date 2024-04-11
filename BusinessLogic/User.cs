using System.Text.RegularExpressions;

namespace BusinessLogic;

public class User
{
    public string Name { get; }
    private string _email;
    public string Email
    {
        get => _email;
        set
        {
            if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Email format is invalid.");
            }

            _email = value;
        }
    }

    public string Password { get; }

    public User(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
}