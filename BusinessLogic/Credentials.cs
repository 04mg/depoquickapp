namespace BusinessLogic;

public readonly struct Credentials
{
    public string Email { init; get; }
    public string Password { init; get; }

    internal Credentials(string email, string password)
    {
        Email = email;
        Password = password;
    }
}