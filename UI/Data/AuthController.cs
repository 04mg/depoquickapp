using BusinessLogic;

namespace UI.Data;

public class AuthController
{
    private readonly DepoQuickApp _app;
    private Credentials? _currentCredentials;

    public Credentials CurrentCredentials
    {
        get => _currentCredentials ?? throw new NullReferenceException();
        private set => _currentCredentials = value;
    }

    public AuthController(DepoQuickApp app)
    {
        _app = app;
    }

    public void LogIn(LoginDto loginDto) => CurrentCredentials = _app.Login(loginDto);

    public void Register(RegisterDto registerDto) => _app.RegisterUser(registerDto);

    public void LogOut() => _currentCredentials = null;
    
    public bool IsLoggedIn => _currentCredentials != null;
    
    public bool IsAdmin => CurrentCredentials.Rank == "Administrator";
}