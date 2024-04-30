using BusinessLogic.Exceptions;

namespace BusinessLogic;

public class AuthManager
{
    private Dictionary<string, User> UsersByEmail { set; get; } = new();
    private bool IsAdminRegistered { set; get; }

    public bool Exists(string email)
    {
        return UsersByEmail.ContainsKey(email);
    }

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

    private void EnsureSingleAdmin(UserRank rank)
    {
        if (rank == UserRank.Administrator && IsAdminRegistered)
        {
            throw new ArgumentException("There can only be one administrator.");
        }
    }

    private void SetAdminRegisteredIfAdmin(UserRank rank)
    {
        if (rank == UserRank.Administrator)
        {
            IsAdminRegistered = true;
        }
    }

    private void ValidateRegistration(User user, string passwordConfirmation)
    {
        EnsureUserIsNotRegistered(user.Email);
        EnsurePasswordConfirmationMatch(user.Password, passwordConfirmation);
        EnsureSingleAdmin(user.Rank);
        SetAdminRegisteredIfAdmin(user.Rank);
    }

    private void ValidateLogin(string email, string password)
    {
        EnsureUserIsRegistered(email);
        EnsurePasswordMatchWithEmail(email, password);
    }

    public Credentials Register(User user, string passwordConfirmation)
    {
        ValidateRegistration(user, passwordConfirmation);

        UsersByEmail.Add(user.Email, user);

        return new Credentials{Email = user.Email, Rank = user.Rank.ToString()};
    }

    public Credentials Login(LoginDto loginDto)
    {
        ValidateLogin(loginDto.Email, loginDto.Password);

        var userRank = UsersByEmail[loginDto.Email].Rank;
        var credentials = new Credentials { Email = loginDto.Email, Rank = userRank.ToString() };
        return credentials;
    }

    public User GetUserByEmail(string email, Credentials credentials)
    {
        if (!Exists(email))
        {
            throw new ArgumentException("User does not exist.");
        }

        EnsureUserIsAdminOrSameUser(email, credentials);
        return UsersByEmail[email];
    }

    private void EnsureUserIsAdminOrSameUser(string requestedEmail, Credentials credentials)
    {
        if (credentials.Rank == "Administrator") return;
        if (credentials.Email != requestedEmail)
        {
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
        }
    }
}