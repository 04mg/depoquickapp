using BusinessLogic.Domain;
using BusinessLogic.DTOs;

namespace BusinessLogic.Managers;

public class DepositManager
{
    private List<Deposit> Deposits { get; } = new();

    public void EnsureDepositExists(int id)
    {
        if (Deposits.All(d => d.Id != id))
        {
            throw new ArgumentException("Deposit not found.");
        }
    }

    private static void EnsureUserIsAdmin(Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
        {
            throw new UnauthorizedAccessException("Only administrators can manage deposits.");
        }
    }

    public Deposit GetDepositById(int id)
    {
        return Deposits.First(d => d.Id == id);
    }

    public void Add(Deposit deposit, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        deposit.Id = NextDepositId;
        Deposits.Add(deposit);
    }

    public void Delete(int id, Credentials credentials)
    {
        EnsureDepositExists(id);
        EnsureUserIsAdmin(credentials);

        var deposit = GetDepositById(id);
        Deposits.Remove(deposit);
    }

    private int NextDepositId => Deposits.Count > 0 ? Deposits.Max(d => d.Id) + 1 : 1;

    public List<Deposit> GetAllDeposits()
    {
        return Deposits;
    }

    public void EnsureThereAreNoDepositsWithThisPromotion(int id, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        if (Deposits.Any(d => d.Promotions.Any(p => p.Id == id)))
        {
            throw new ArgumentException("There are existing deposits for this promotion.");
        }
    }
}