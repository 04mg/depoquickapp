using BusinessLogic.DTOs;
using BusinessLogic.Exceptions;
using DataAccess.Interfaces;
using Domain;
using Domain.Enums;

namespace BusinessLogic.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private Credentials? _currentCredentials;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Credentials CurrentCredentials =>
        _currentCredentials ?? throw new BusinessLogicException("User is not logged in.");

    public void Register(RegisterDto registerDto)
    {
        var user = new User(
            registerDto.NameSurname,
            registerDto.Email,
            registerDto.Password,
            registerDto.Rank);
        EnsureUserIsNotRegistered(user.Email);
        EnsurePasswordConfirmationMatch(user.Password, registerDto.PasswordConfirmation);
        EnsureSingleAdmin(user.Rank);
        SetRankAsAdminIfFirstUser(user);
        _userRepository.Add(user);
        var credentials = new Credentials() { Email = user.Email, Rank = user.Rank.ToString() };
        _currentCredentials = credentials;
    }

    private static void EnsurePasswordConfirmationMatch(string password, string passwordConfirmation)
    {
        if (password != passwordConfirmation) throw new BusinessLogicException("Passwords do not match.");
    }

    private void EnsurePasswordMatchWithEmail(string email, string password)
    {
        var user = _userRepository.Get(email);
        if (user.Password != password) throw new BusinessLogicException("Wrong password.");
    }

    private void EnsureUserIsRegistered(string email)
    {
        if (!_userRepository.Exists(email)) throw new BusinessLogicException("User does not exist.");
    }

    private void EnsureUserIsNotRegistered(string email)
    {
        if (_userRepository.Exists(email)) throw new BusinessLogicException("User already exists.");
    }

    private void EnsureSingleAdmin(UserRank rank)
    {
        if (rank == UserRank.Administrator && _userRepository.GetAll().Any())
            throw new BusinessLogicException("There can only be one administrator.");
    }

    private void SetRankAsAdminIfFirstUser(User user)
    {
        if (!_userRepository.GetAll().Any()) user.Rank = UserRank.Administrator;
    }

    public void Login(LoginDto loginDto)
    {
        EnsureUserIsRegistered(loginDto.Email);
        EnsurePasswordMatchWithEmail(loginDto.Email, loginDto.Password);
        var user = _userRepository.Get(loginDto.Email);
        var credentials = new Credentials { Email = loginDto.Email, Rank = user.Rank.ToString() };
        _currentCredentials = credentials;
    }

    public void Logout() => _currentCredentials = null;

    public bool IsLoggedIn() => _currentCredentials != null;

    public bool IsAdmin() => _currentCredentials?.Rank == UserRank.Administrator.ToString();
}