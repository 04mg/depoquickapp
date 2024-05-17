using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Enums;

namespace BusinessLogic.Managers;

public class AuthManager
{
    private Dictionary<string, User> UsersByEmail { get; } = new();
    private bool IsAdminRegistered { set; get; }

    public bool Exists(string email)
    {
        return UsersByEmail.ContainsKey(email);
    }

    private static void EnsurePasswordConfirmationMatch(string password, string passwordConfirmation)
    {
        if (password != passwordConfirmation) throw new ArgumentException("Passwords do not match.");
    }

    private void EnsurePasswordMatchWithEmail(string email, string password)
    {
        if (UsersByEmail[email].Password != password) throw new ArgumentException("Wrong password.");
    }

    private void EnsureUserIsRegistered(string email)
    {
        if (!UsersByEmail.ContainsKey(email)) throw new ArgumentException("User does not exist.");
    }

    private void EnsureUserIsNotRegistered(string email)
    {
        if (UsersByEmail.ContainsKey(email)) throw new ArgumentException("User already exists.");
    }

    private void EnsureSingleAdmin(UserRank rank)
    {
        if (rank == UserRank.Administrator && IsAdminRegistered)
            throw new ArgumentException("There can only be one administrator.");
    }

    private void SetAdminRegisteredIfAdmin(UserRank rank)
    {
        if (rank == UserRank.Administrator) IsAdminRegistered = true;
    }

    public Credentials Register(User user, string passwordConfirmation)
    {
        SetRankAsAdminIfFirstUser(user);
        EnsureUserIsNotRegistered(user.Email);
        EnsurePasswordConfirmationMatch(user.Password, passwordConfirmation);
        EnsureSingleAdmin(user.Rank);
        SetAdminRegisteredIfAdmin(user.Rank);
        UsersByEmail.Add(user.Email, user);
        return new Credentials { Email = user.Email, Rank = user.Rank.ToString() };
    }

    private void SetRankAsAdminIfFirstUser(User user)
    {
        if (UsersByEmail.Count == 0) user.Rank = UserRank.Administrator;
    }

    public Credentials Login(LoginDto loginDto)
    {
        EnsureUserIsRegistered(loginDto.Email);
        EnsurePasswordMatchWithEmail(loginDto.Email, loginDto.Password);
        var userRank = UsersByEmail[loginDto.Email].Rank;
        var credentials = new Credentials { Email = loginDto.Email, Rank = userRank.ToString() };
        return credentials;
    }

    public User GetUserByEmail(string email, Credentials credentials)
    {
        if (!Exists(email)) throw new ArgumentException("User does not exist.");

        EnsureUserIsAdminOrSameUser(email, credentials);
        return UsersByEmail[email];
    }

    private static void EnsureUserIsAdminOrSameUser(string requestedEmail, Credentials credentials)
    {
        if (credentials.Rank != "Administrator" && credentials.Email != requestedEmail)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
    }
}