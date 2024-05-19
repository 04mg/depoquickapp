using BusinessLogic.Domain;
using BusinessLogic.DTOs;

namespace BusinessLogic.Managers;

public class DepositManager
{
    private List<Deposit> Deposits { get; } = new();

    public void EnsureDepositExists(string name)
    {
        name = name.ToLower();
        if (Deposits.All(d => d.Name.ToLower() != name)) throw new ArgumentException("Deposit not found.");
    }

    private static void EnsureUserIsAdmin(Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
            throw new UnauthorizedAccessException("Only administrators can manage deposits.");
    }

    public Deposit GetDepositById(string name)
    {
        name = name.ToLower();
        return Deposits.First(d => d.Name.ToLower() == name);
    }

    public void Add(Deposit deposit, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsureDepositNameIsNotTaken(deposit.Name);
        Deposits.Add(deposit);
    }

    private void EnsureDepositNameIsNotTaken(string depositName)
    {
        depositName = depositName.ToLower();
        if (Deposits.Any(d => d.Name.ToLower() == depositName))
            throw new ArgumentException("Deposit name is already taken.");
    }

    public void Delete(string name, Credentials credentials)
    {
        EnsureDepositExists(name);
        EnsureUserIsAdmin(credentials);

        var deposit = GetDepositById(name);
        Deposits.Remove(deposit);
    }

    public List<Deposit> GetAllDeposits()
    {
        return Deposits;
    }

    public void EnsureThereAreNoDepositsWithThisPromotion(int id, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        if (Deposits.Any(d => d.HasPromotion(id)))
            throw new ArgumentException("There are existing deposits for this promotion.");
    }
}