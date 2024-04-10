using BusinessLogic.Exceptions;

namespace BusinessLogic;

public class CredentialsManager
{
    private List<Credentials> CredentialsList { set; get; }

    public CredentialsManager()
    {
        CredentialsList = new List<Credentials>();
    }

    public bool IsRegistered(string email)
    {
        return CredentialsList.Any(c => c.Email == email);
    }

    private bool PasswordConfirmationMatches(string password, string passwordConfirmation)
    {
        return password == passwordConfirmation;
    }

    private bool EmailPasswordMatches(string email, string password)
    {
        return CredentialsList.Any(c => c.Email == email && c.Password == password);
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

        CredentialsList.Add(new Credentials(email, password));
    }

    public Credentials Login(string email, string password)
    {
        EnsureUserIsRegistered(email);
        EnsureEmailPasswordMatches(email, password);

        return CredentialsList.Find(c => c.Email == email && c.Password == password);
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