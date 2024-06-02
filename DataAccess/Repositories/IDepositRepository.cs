using Domain;

namespace DataAccess.Repositories;

public interface IDepositRepository : IRepository<string, Deposit>
{
    IEnumerable<Deposit> GetAll();
    bool Exists(string name);
    void Delete(string name);
}