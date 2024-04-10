using BusinessLogic.Exceptions;

namespace BusinessLogic;

public class CredentialsManager
{
    private List<Credentials> CredentialsList { set; get; }

    public CredentialsManager()
    {
        CredentialsList = new List<Credentials>();
    }

    private bool PasswordsMatch(string password, string passwordConfirmation)
    {
        return password == passwordConfirmation;
    }

    public bool IsRegistered(string email)
    {
        return CredentialsList.Any(c => c.Email == email);
    }

    public void Register(string email, string password, string passwordConfirmation)
    {
        if (IsRegistered(email))
        {
            throw new UserAlreadyExistsException("User already exists.");
        }
        else if (!PasswordsMatch(password, passwordConfirmation))
        {
            throw new ArgumentException("Passwords do not match.");
        }

        CredentialsList.Add(new Credentials(email, password));
    }

    public Credentials Login(string email, string password)
    {
        if (CredentialsList.Any(c => c.Email == email))
        {
            var user = CredentialsList.Find(c => c.Email == email);
            if (user.Password != password)
            {
                throw new ArgumentException("Wrong password.");
            }
        }
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