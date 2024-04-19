namespace BusinessLogic;

public readonly struct Credentials
{
    public string Email { init; get; }
    public string Rank { init; get; }

    internal Credentials(string email, string rank)
    {
        Email = email;
        Rank = rank;
    }
}