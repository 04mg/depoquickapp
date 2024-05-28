using BusinessLogic.Domain;

namespace BusinessLogic.Repositories;

public interface IPromotionRepository : IRepository<int, Promotion>
{
    bool Exists(int id);
    void Modify(int id, Promotion newPromotion);
    void Delete(int id);
    IEnumerable<Promotion> GetAll();
}