namespace BusinessLogic;

public readonly struct Credentials
{
    public string Email { init; get; }
    public UserRank Rank { init; get; }

    internal Credentials(string email, UserRank rank)
    {
        Email = email;
        Rank = rank;
    }
}