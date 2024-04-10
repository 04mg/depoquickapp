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

    public void Register(string email, string password, string passwordConfirmation)
    {
        CredentialsList.Add(new Credentials(email, password));
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