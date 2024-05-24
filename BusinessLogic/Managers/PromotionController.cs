using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Repositories;

namespace BusinessLogic.Managers;

public class PromotionController
{
    private readonly IPromotionRepository _promotionRepository;

    public PromotionController()
    {
        _promotionRepository = new PromotionRepository();
    }

    private static void EnsureUserIsAdmin(Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
            throw new UnauthorizedAccessException("Only administrators can manage promotions.");
    }

    public void EnsurePromotionExists(int id)
    {
        if (!_promotionRepository.Exists(id)) throw new ArgumentException("Promotion not found.");
    }

    public Promotion GetPromotion(int id)
    {
        return _promotionRepository.Get(id);
    }
    
    public bool Exists(int id)
    {
        return _promotionRepository.Exists(id);
    }

    public void Add(Promotion promotion, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        _promotionRepository.Add(promotion);
    }

    public void Delete(int id, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsurePromotionExists(id);
        _promotionRepository.Delete(id);
    }

    public void Modify(int id, Promotion newPromotion, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsurePromotionExists(id);
        _promotionRepository.Modify(id, newPromotion);
    }

    public IEnumerable<Promotion> GetAllPromotions(Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        return _promotionRepository.GetAll();
    }
}