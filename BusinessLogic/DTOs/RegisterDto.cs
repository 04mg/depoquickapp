namespace BusinessLogic.DTOs;

public struct RegisterDto
{
    public string NameSurname { set; get; }
    public string Email { set; get; }
    public string Password { set; get; }
    public string PasswordConfirmation { set; get; }
    public string Rank { init; get; }
}