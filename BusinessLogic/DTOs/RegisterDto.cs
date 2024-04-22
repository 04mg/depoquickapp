namespace BusinessLogic;

public readonly struct RegisterDto
{
    public string NameSurname { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public string PasswordConfirmation { get; init; }
    public string Rank { get; init; }

    public RegisterDto(string nameSurname, string email, string password, string passwordConfirmation, string rank = "Client")
    {
        NameSurname = nameSurname;
        Email = email;
        Password = password;
        PasswordConfirmation = passwordConfirmation;
        Rank = rank;
    }
}