using BusinessLogic.Domain;

namespace BusinessLogic.Repositories;

public class DepositRepository : IDepositRepository
{
    private readonly List<Deposit> _deposits = new();

    public void Add(Deposit deposit)
    {
        _deposits.Add(deposit);
    }

    public Deposit Get(string name)
    {
        return _deposits.First(d => string.Equals(d.Name, name, StringComparison.CurrentCultureIgnoreCase));
    }

    public void Delete(string name)
    {
        var deposit = Get(name);
        _deposits.Remove(deposit);
    }

    public IEnumerable<Deposit> GetAll()
    {
        return _deposits;
    }

    public bool Exists(string name)
    {
        return _deposits.Any(d => string.Equals(d.Name, name, StringComparison.CurrentCultureIgnoreCase));
    }
}