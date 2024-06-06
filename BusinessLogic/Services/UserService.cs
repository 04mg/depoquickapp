using BusinessLogic.DTOs;
using DataAccess.Repositories;
using Domain;
using Domain.Enums;

namespace BusinessLogic.Services;

public class UserService
{
    private bool _isAdminRegistered;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Credentials Register(RegisterDto registerDto)
    {
        var user = new User(
            registerDto.NameSurname,
            registerDto.Email,
            registerDto.Password,
            registerDto.Rank);
        SetRankAsAdminIfFirstUser(user);
        EnsureUserIsNotRegistered(user.Email);
        EnsurePasswordConfirmationMatch(user.Password, registerDto.PasswordConfirmation);
        EnsureSingleAdmin(user.Rank);
        SetAdminRegisteredIfAdmin(user.Rank);
        _userRepository.Add(user);
        return new Credentials { Email = user.Email, Rank = user.Rank.ToString() };
    }

    private static void EnsurePasswordConfirmationMatch(string password, string passwordConfirmation)
    {
        if (password != passwordConfirmation) throw new ArgumentException("Passwords do not match.");
    }

    private void EnsurePasswordMatchWithEmail(string email, string password)
    {
        var user = _userRepository.Get(email);
        if (user.Password != password) throw new ArgumentException("Wrong password.");
    }

    private void EnsureUserIsRegistered(string email)
    {
        if (!_userRepository.Exists(email)) throw new ArgumentException("User does not exist.");
    }

    private void EnsureUserIsNotRegistered(string email)
    {
        if (_userRepository.Exists(email)) throw new ArgumentException("User already exists.");
    }

    private void EnsureSingleAdmin(UserRank rank)
    {
        if (rank == UserRank.Administrator && _isAdminRegistered)
            throw new ArgumentException("There can only be one administrator.");
    }

    private void SetAdminRegisteredIfAdmin(UserRank rank)
    {
        if (rank == UserRank.Administrator) _isAdminRegistered = true;
    }

    private void SetRankAsAdminIfFirstUser(User user)
    {
        if (!_isAdminRegistered) user.Rank = UserRank.Administrator;
    }

    public Credentials Login(LoginDto loginDto)
    {
        EnsureUserIsRegistered(loginDto.Email);
        EnsurePasswordMatchWithEmail(loginDto.Email, loginDto.Password);
        var user = _userRepository.Get(loginDto.Email);
        var credentials = new Credentials { Email = loginDto.Email, Rank = user.Rank.ToString() };
        return credentials;
    }
}