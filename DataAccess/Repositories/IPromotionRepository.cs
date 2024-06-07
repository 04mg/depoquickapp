using Domain;

namespace DataAccess.Repositories;

public interface IPromotionRepository : IRepository<int, Promotion>
{
    bool Exists(int id);
    void Update(Promotion promotion);
    void Delete(int id);
    IEnumerable<Promotion> GetAll();
}