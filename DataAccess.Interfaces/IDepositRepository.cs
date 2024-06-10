using Domain;

namespace DataAccess.Interfaces;

public interface IDepositRepository
{
    void Add(Deposit deposit);
    Deposit Get(string name);
    IEnumerable<Deposit> GetAll();
    bool Exists(string name);
    void Delete(string name);
    void Update(Deposit deposit);
}