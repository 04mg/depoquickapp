using System.Text.RegularExpressions;

namespace BusinessLogic;

public class User
{
    private static readonly Regex EmailRegex = new(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
    public string Name { get; }
    private string _email;
    private string _password;
    public string Email
    {
        get => _email;
        set
        {
            EnsureEmailIsValid(value);
            _email = value;
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            var symbols = "#@$.,%";
            if(!value.Any(symbols.Contains))
            {
                throw new ArgumentException(
                    "Password format is invalid, it must contain at least one of the following symbols: #@$.,%");
            }
            if(value.Length < 8)
            {
                throw new ArgumentException("Password format is invalid, length must be at least 8.");
            }
            if(!value.Any(char.IsUpper))
            {
                throw new ArgumentException("Password format is invalid, it must contain at least one uppercase letter.");
            }

            if (!value.Any(char.IsLower))
            {
                throw new ArgumentException("Password format is invalid, it must contain at least one lowercase letter.");
            }
            if (!value.Any(char.IsDigit))
            {
                throw new ArgumentException("Password format is invalid, it must contain at least one digit.");
            }
            
            _password = value;
        }
    }

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