using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Enums;
using BusinessLogic.Repositories;

namespace BusinessLogic.Logic;

public class AuthLogic
{
    private bool _isAdminRegistered;
    private readonly IUserRepository _userRepository;

    public AuthLogic(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Credentials Register(User user, string passwordConfirmation)
    {
        SetRankAsAdminIfFirstUser(user);
        EnsureUserIsNotRegistered(user.Email);
        EnsurePasswordConfirmationMatch(user.Password, passwordConfirmation);
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

    public User GetUser(string email, Credentials credentials)
    {
        EnsureUserIsRegistered(email);
        EnsureUserIsAdminOrSameUser(email, credentials);
        return _userRepository.Get(email);
    }

    private static void EnsureUserIsAdminOrSameUser(string requestedEmail, Credentials credentials)
    {
        if (credentials.Rank != "Administrator" && credentials.Email != requestedEmail)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
    }
}