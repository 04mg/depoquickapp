using BusinessLogic.Exceptions;

namespace BusinessLogic;

public class CredentialsManager
{
    private Dictionary<string, Credentials> CredentialsByEmail { set; get; }

    public CredentialsManager()
    {
        CredentialsByEmail = new Dictionary<string, Credentials>();
    }

    public bool IsRegistered(string email)
    {
        return CredentialsByEmail.ContainsKey(email);
    }

    private bool PasswordConfirmationMatches(string password, string passwordConfirmation)
    {
        return password == passwordConfirmation;
    }

    private bool EmailPasswordMatches(string email, string password)
    {
        return CredentialsByEmail[email].Password == password;
    }

    private void EnsurePasswordConfirmationMatches(string password, string passwordConfirmation)
    {
        if (!PasswordConfirmationMatches(password, passwordConfirmation))
        {
            throw new ArgumentException("Passwords do not match.");
        }
    }

    private void EnsureEmailPasswordMatches(string email, string password)
    {
        if (!EmailPasswordMatches(email, password))
        {
            throw new ArgumentException("Wrong password.");
        }
    }

    private void EnsureUserIsRegistered(string email)
    {
        if (!IsRegistered(email))
        {
            throw new ArgumentException("User does not exist.");
        }
    }

    private void EnsureUserIsNotRegistered(string email)
    {
        if (IsRegistered(email))
        {
            throw new UserAlreadyExistsException("User already exists.");
        }
    }

    public void Register(string email, string password, string passwordConfirmation)
    {
        EnsureUserIsNotRegistered(email);
        EnsurePasswordConfirmationMatches(password, passwordConfirmation);

        CredentialsByEmail.Add(email, new Credentials(email, password));
    }

    public Credentials Login(string email, string password)
    {
        EnsureUserIsRegistered(email);
        EnsureEmailPasswordMatches(email, password);

        return CredentialsByEmail[email];
    }
}

public readonly struct Credentials
{
    public string Email { init; get; }
    public string Password { init; get; }

    public Credentials(string email, string password)
    {
        Email = email;
        Password = password;
    }
}