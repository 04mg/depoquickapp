using BusinessLogic.Exceptions;

namespace BusinessLogic;

public class AuthManager
{
    private Dictionary<string, User> UsersByEmail { set; get; }

    public AuthManager()
    {
        UsersByEmail = new Dictionary<string, User>();
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

    public Credentials Register(UserModel userModel, string passwordConfirmation)
    {
        EnsureUserIsNotRegistered(userModel.Email);
        EnsurePasswordConfirmationMatch(userModel.Password, passwordConfirmation);
        
        var user = new User(userModel.NameSurname, userModel.Email, userModel.Password);
        UsersByEmail.Add(userModel.Email, user);
        
        var credentials = new Credentials(userModel.Email, userModel.Password);
        return credentials;
    }

    public Credentials Login(string email, string password)
    {
        EnsureUserIsRegistered(email);
        EnsurePasswordMatchWithEmail(email, password);

        var credentials = new Credentials(email, password);
        return credentials;
    }
}