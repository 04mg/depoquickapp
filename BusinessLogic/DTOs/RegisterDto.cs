namespace BusinessLogic;

public struct RegisterDto
{
    public string NameSurname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
    public string Rank { get; set; }

    public RegisterDto(string nameSurname, string email, string password, string passwordConfirmation, string rank = "Client")
    {
        NameSurname = nameSurname;
        Email = email;
        Password = password;
        PasswordConfirmation = passwordConfirmation;
        Rank = rank;
    }
}