using System.Text.RegularExpressions;

namespace BusinessLogic;

public class User
{
    private static readonly Regex EmailRegex = new(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
    private string _name;
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
            EnsurePasswordHasSymbol(value);
            EnsurePasswordLengthGreaterThan8(value);
            EnsurePasswordHasUppercaseLetter(value);
            EnsurePasswordHasLowercaseLetter(value);
            EnsurePasswordHasDigit(value);
            _password = value;
        }
    }
    
    public string Name
    {
        get => _name;
        set
        {
            if (!value.Contains(' '))
            {
                throw new ArgumentException("NameSurname format is invalid, it has to contain a space between the name and surname.");
            }
            var name = value.Split(' ');
            if (name.Length < 2 || name[0].Equals("") || name[1].Equals(""))
            {
                throw new ArgumentException("NameSurname format is invalid, it has to contain a name and a surname.");
            }
            
            _name = value;
        }
    }

    private static void EnsurePasswordHasDigit(string password)
    {
        if (!password.Any(char.IsDigit))
        {
            throw new ArgumentException("Password format is invalid, it must contain at least one digit.");
        }
    }

    private static void EnsurePasswordHasLowercaseLetter(string password)
    {
        if (!password.Any(char.IsLower))
        {
            throw new ArgumentException("Password format is invalid, it must contain at least one lowercase letter.");
        }
    }

    private static void EnsurePasswordHasUppercaseLetter(string password)
    {
        if(!password.Any(char.IsUpper))
        {
            throw new ArgumentException("Password format is invalid, it must contain at least one uppercase letter.");
        }
    }

    private static void EnsurePasswordLengthGreaterThan8(string password)
    {
        if(password.Length < 8)
        {
            throw new ArgumentException("Password format is invalid, length must be at least 8.");
        }
    }

    private static void EnsurePasswordHasSymbol(string password)
    {
        var symbols = "#@$.,%";
        if(!password.Any(symbols.Contains))
        {
            throw new ArgumentException(
                "Password format is invalid, it must contain at least one of the following symbols: #@$.,%");
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