namespace BusinessLogic;

public readonly struct LoginDto
{
    public string Email { get; init; }
    public string Password { get; init; }
}