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

    private void EnsureSingleAdmin(UserRank rank)
    {
        if (rank.Equals(UserRank.Administrator) && IsAdminRegistered)
        {
            throw new ArgumentException("There can only be one administrator.");
        }
    }

    private void SetAdminRegisteredIfAdmin(UserRank rank)
    {
        if (rank.Equals(UserRank.Administrator))
        {
            IsAdminRegistered = true;
        }
    }

    private void ValidateRegistration(UserModel userModel, string passwordConfirmation)
    {
        EnsureUserIsNotRegistered(userModel.Email);
        EnsurePasswordConfirmationMatch(userModel.Password, passwordConfirmation);
        EnsureSingleAdmin(userModel.Rank);
        SetAdminRegisteredIfAdmin(userModel.Rank);
    }

    private void ValidateLogin(string email, string password)
    {
        EnsureUserIsRegistered(email);
        EnsurePasswordMatchWithEmail(email, password);
    }

    public Credentials Register(UserModel userModel, string passwordConfirmation)
    {
        ValidateRegistration(userModel, passwordConfirmation);

        var user = new User(userModel.NameSurname, userModel.Email, userModel.Password, userModel.Rank);
        UsersByEmail.Add(userModel.Email, user);

        return new Credentials(userModel.Email, userModel.Rank.ToString());
    }

    public Credentials Login(string email, string password)
    {
        ValidateLogin(email, password);

        var userRank = UsersByEmail[email].Rank;
        var credentials = new Credentials(email, userRank.ToString());
        return credentials;
    }
}