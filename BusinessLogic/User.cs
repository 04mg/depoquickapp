using System.Text.RegularExpressions;

namespace BusinessLogic;

public class User
{
    private string _nameSurname = "";
    private string _email = "";
    private string _password = "";

    public string Email
    {
        get => _email;
        set
        {
            EnsureEmailIsValidFormat(value);
            EnsureEmailHasValidLength(value);
            _email = value;
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            EnsurePasswordHasSymbol(value);
            EnsurePasswordHasValidLength(value);
            EnsurePasswordHasUppercaseLetter(value);
            EnsurePasswordHasLowercaseLetter(value);
            EnsurePasswordHasDigit(value);
            _password = value;
        }
    }

    public string NameSurname
    {
        get => _nameSurname;
        set
        {
            EnsureNameSurnameContainsSpace(value);
            EnsureNameSurnameHasNameAndSurname(value);
            EnsureNameSurnameHasValidLength(value);
            EnsureNameSurnameHasOnlyLettersAndWhitespaces(value);
            _nameSurname = value;
        }
    }

    private static void EnsureNameSurnameHasOnlyLettersAndWhitespaces(string value)
    {
        if (!value.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
        {
            throw new ArgumentException("NameSurname format is invalid, it should only contain letters and whitespaces.");
        }
    }
    
    public UserRank Rank { get; }

    public User(string nameSurname, string email, string password, UserRank rank = UserRank.Client)
    {
        NameSurname = nameSurname;
        Email = email;
        Password = password;
        Rank = rank;
    }

    private static void EnsureNameSurnameContainsSpace(string nameSurname)
    {
        if (!nameSurname.Contains(' '))
        {
            throw new ArgumentException(
                "NameSurname format is invalid, it has to contain a space between the name and surname.");
        }
    }

    private static void EnsureNameSurnameHasValidLength(string nameSurname)
    {
        if (nameSurname.Length > 100)
        {
            throw new ArgumentException("NameSurname format is invalid, length must be lesser or equal to 100.");
        }
    }

    private static void EnsureNameSurnameHasNameAndSurname(string nameSurname)
    {
        var nameSurnameParts = nameSurname.Split(' ');
        if (nameSurnameParts.Length < 2 || string.IsNullOrWhiteSpace(nameSurnameParts[0]) ||
            string.IsNullOrWhiteSpace(nameSurnameParts[1]))
        {
            throw new ArgumentException("NameSurname format is invalid, it has to contain a name and a surname.");
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
        if (!password.Any(char.IsUpper))
        {
            throw new ArgumentException("Password format is invalid, it must contain at least one uppercase letter.");
        }
    }

    private static void EnsurePasswordHasValidLength(string password)
    {
        if (password.Length < 8)
        {
            throw new ArgumentException("Password format is invalid, length must be at least 8.");
        }
    }

    private static void EnsurePasswordHasSymbol(string password)
    {
        const string symbols = "#@$.,%";
        if (!password.Any(symbols.Contains))
        {
            throw new ArgumentException(
                "Password format is invalid, it must contain at least one of the following symbols: #@$.,%");
        }
    }

    private static void EnsureEmailIsValidFormat(string email)
    {
        Regex emailRegex = new(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        if (!emailRegex.IsMatch(email))
        {
            throw new ArgumentException("Email format is invalid.");
        }
    }

    private static void EnsureEmailHasValidLength(string email)
    {
        if (email.Length > 254)
        {
            throw new ArgumentException("Email format is invalid, length must be lesser or equal to 254.");
        }
    }
}

public enum UserRank
{
    Client,
    Administrator
}