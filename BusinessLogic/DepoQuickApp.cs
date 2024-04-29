namespace BusinessLogic;

public class DepoQuickApp
{
    private AuthManager _authManager;
    public DepoQuickApp()
    {
        _authManager = new AuthManager();
    }
    
    public void RegisterUser(RegisterDto registerDto)
    {
        var user = new User(
            registerDto.NameSurname,
            registerDto.Email, 
            registerDto.Password,
            registerDto.Rank);
        _authManager.Register(user, registerDto.PasswordConfirmation);
    }
    
    public Credentials Login(LoginDto loginDto)
    {
        return _authManager.Login(loginDto);
    }
}


