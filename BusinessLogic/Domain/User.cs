using System.Text.RegularExpressions;

namespace BusinessLogic.Domain;

public class User
{
    private string _nameSurname = "";
    private string _email = "";
    private string _password = "";
    private const int MaxEmailLength = 254;
    private const int MaxNameSurnameLength = 100;
    private const int MinPasswordLength = 8;
    private const string ValidSymbols = "#@$.,%";
    private static readonly Regex EmailRegex = new(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");


    public string Email
    {
        get => _email;
        private set
        {
            EnsureEmailIsValidFormat(value);
            EnsureEmailHasValidLength(value);
            _email = value;
        }
    }

    public string Password
    {
        get => _password;
        private set
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
        private set
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
            throw new ArgumentException(
                "NameSurname format is invalid, it should only contain letters and whitespaces.");
        }
    }

    public UserRank Rank { get; set; }

    public User(string nameSurname, string email, string password, string rank = "Client")
    {
        NameSurname = nameSurname;
        Email = email;
        Password = password;
        EnsureRankIsValid(rank);
        Rank = Enum.Parse<UserRank>(rank);
    }

    private static void EnsureRankIsValid(string rank)
    {
        if (!Enum.TryParse<UserRank>(rank, out _))
        {
            throw new ArgumentException("Rank is invalid.");
        }
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
        if (nameSurname.Length > MaxNameSurnameLength)
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
        if (password.Length < MinPasswordLength)
        {
            throw new ArgumentException("Password format is invalid, length must be at least 8.");
        }
    }

    private static void EnsurePasswordHasSymbol(string password)
    {
        if (!password.Any(ValidSymbols.Contains))
        {
            throw new ArgumentException(
                "Password format is invalid, it must contain at least one of the following symbols: #@$.,%");
        }
    }

    private static void EnsureEmailIsValidFormat(string email)
    {
        if (!EmailRegex.IsMatch(email))
        {
            throw new ArgumentException("Email format is invalid.");
        }
    }

    private static void EnsureEmailHasValidLength(string email)
    {
        if (email.Length > MaxEmailLength)
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