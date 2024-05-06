namespace BusinessLogic.DTOs;

public struct RegisterDto
{
    public string NameSurname { set; get; }
    public string Email { set; get; }
    public string Password { set; get; }
    public string PasswordConfirmation { set; get; }
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