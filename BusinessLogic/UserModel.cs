namespace BusinessLogic;

public readonly struct UserModel
{
    public string NameSurname { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public UserRank Rank { get; init; }
    
    public UserModel(string nameSurname, string email, string password, UserRank rank = UserRank.Client)
    {
        NameSurname = nameSurname;
        Email = email;
        Password = password;
        Rank = rank;
    }
}