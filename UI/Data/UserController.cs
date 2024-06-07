using BusinessLogic.DTOs;
using BusinessLogic.Services;

namespace UI.Data;

public class UserController
{
    private readonly UserService _userService;
    private Credentials? _currentCredentials;
    public event Action? OnLoginStatusChanged;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    public Credentials CurrentCredentials
    {
        get => _currentCredentials ?? throw new NullReferenceException();
        private set => _currentCredentials = value;
    }

    public bool IsLoggedIn => _currentCredentials != null;

    public bool IsAdmin => CurrentCredentials.Rank == "Administrator";

    public void LogIn(LoginDto loginDto)
    {
        CurrentCredentials = _userService.Login(loginDto);
        OnLoginStatusChanged?.Invoke();
    }

    public void Register(RegisterDto registerDto)
    {
        _userService.Register(registerDto);
    }

    public void LogOut()
    {
        _currentCredentials = null;
        OnLoginStatusChanged?.Invoke();
    }
}