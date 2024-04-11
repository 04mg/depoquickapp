using System.Text.RegularExpressions;

namespace BusinessLogic;

public class User
{
    private static readonly Regex EmailRegex = new(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
    public string Name { get; }
    private string _email;
    public string Email
    {
        get => _email;
        set
        {
            EnsureEmailIsValid(value);
            _email = value;
        }
    }

    public string Password { get; }

    private bool EmailIsValid(string email)
    {
        return EmailRegex.IsMatch(email);
    }
    
    private void EnsureEmailIsValid(string email)
    {
        if (!EmailIsValid(email))
        {
            throw new ArgumentException("Email format is invalid.");
        }
    }
    public User(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
}