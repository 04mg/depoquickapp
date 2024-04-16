using BusinessLogic.Exceptions;

namespace BusinessLogic;

public class CredentialsManager
{
    private Dictionary<string, Credentials> CredentialsByEmail { set; get; }

    public CredentialsManager()
    {
        CredentialsByEmail = new Dictionary<string, Credentials>();
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
        if (CredentialsByEmail[email].Password != password)
        {
            throw new ArgumentException("Wrong password.");
        }
    }

    private void EnsureUserIsRegistered(string email)
    {
        if (!CredentialsByEmail.ContainsKey(email))
        {
            throw new ArgumentException("User does not exist.");
        }
    }

    private void EnsureUserIsNotRegistered(string email)
    {
        if (CredentialsByEmail.ContainsKey(email))
        {
            throw new UserAlreadyExistsException("User already exists.");
        }
    }

    public Credentials Register(string email, string password, string passwordConfirmation)
    {
        EnsureUserIsNotRegistered(email);
        EnsurePasswordConfirmationMatch(password, passwordConfirmation);

        var credentials = new Credentials(email, password);
        CredentialsByEmail.Add(email, credentials);
        return credentials;
    }

    public Credentials Login(string email, string password)
    {
        EnsureUserIsRegistered(email);
        EnsurePasswordMatchWithEmail(email, password);

        return CredentialsByEmail[email];
    }
}