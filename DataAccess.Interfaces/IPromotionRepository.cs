using Domain;

namespace DataAccess.Interfaces;

public interface IPromotionRepository
{
    void Add(Promotion promotion);
    Promotion Get(int id);
    bool Exists(int id);
    void Update(Promotion promotion);
    void Delete(int id);
    IEnumerable<Promotion> GetAll();
}