using BusinessLogic.Exceptions;

namespace BusinessLogic;

public class AuthManager
{
    private Dictionary<string, User> UsersByEmail { set; get; } = new();
    private bool IsAdminRegistered { set; get; }

    private static void EnsurePasswordConfirmationMatch(string password, string passwordConfirmation)
    {
        if (password != passwordConfirmation)
        {
            throw new ArgumentException("Passwords do not match.");
        }
    }

    private void EnsurePasswordMatchWithEmail(string email, string password)
    {
        if (UsersByEmail[email].Password != password)
        {
            throw new ArgumentException("Wrong password.");
        }
    }

    private void EnsureUserIsRegistered(string email)
    {
        if (!UsersByEmail.ContainsKey(email))
        {
            throw new ArgumentException("User does not exist.");
        }
    }

    private void EnsureUserIsNotRegistered(string email)
    {
        if (UsersByEmail.ContainsKey(email))
        {
            throw new UserAlreadyExistsException("User already exists.");
        }
    }

    private void EnsureSingleAdmin(string rank)
    {
        if (rank.Equals("Administrator") && IsAdminRegistered)
        {
            throw new ArgumentException("There can only be one administrator.");
        }
    }

    private void SetAdminRegisteredIfAdmin(string rank)
    {
        if (rank.Equals("Administrator"))
        {
            IsAdminRegistered = true;
        }
    }

    private void ValidateRegistration(RegisterDto registerDto)
    {
        EnsureUserIsNotRegistered(registerDto.Email);
        EnsurePasswordConfirmationMatch(registerDto.Password, registerDto.PasswordConfirmation);
        EnsureSingleAdmin(registerDto.Rank);
        SetAdminRegisteredIfAdmin(registerDto.Rank);
    }

    private void ValidateLogin(string email, string password)
    {
        EnsureUserIsRegistered(email);
        EnsurePasswordMatchWithEmail(email, password);
    }

    public Credentials Register(RegisterDto registerDto)
    {
        ValidateRegistration(registerDto);

        var user = new User(registerDto.NameSurname, registerDto.Email, registerDto.Password,
            Enum.Parse<UserRank>(registerDto.Rank));
        UsersByEmail.Add(registerDto.Email, user);

        return new Credentials(registerDto.Email, registerDto.Rank);
    }

    public Credentials Login(string email, string password)
    {
        ValidateLogin(email, password);

        var userRank = UsersByEmail[email].Rank;
        var credentials = new Credentials(email, userRank.ToString());
        return credentials;
    }
}