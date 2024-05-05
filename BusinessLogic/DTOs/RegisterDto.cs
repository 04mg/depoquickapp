namespace BusinessLogic.DTOs;

public struct RegisterDto
{
    public string NameSurname { init; get; }
    public string Email { init; get; }
    public string Password { init; get; }
    public string PasswordConfirmation { init; get; }
    public string Rank { init; get; }

    public RegisterDto(string nameSurname, string email, string password, string passwordConfirmation, string rank = "Client")
    {
        NameSurname = nameSurname;
        Email = email;
        Password = password;
        PasswordConfirmation = passwordConfirmation;
        Rank = rank;
    }
}