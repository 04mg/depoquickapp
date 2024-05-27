using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Repositories;

namespace BusinessLogic.Logic;

public class PromotionLogic
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IDepositRepository _depositRepository;

    public PromotionLogic(IPromotionRepository promotionRepository, IDepositRepository depositRepository)
    {
        _promotionRepository = promotionRepository;
        _depositRepository = depositRepository;
    }

    private static void EnsureUserIsAdmin(Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
            throw new UnauthorizedAccessException("Only administrators can manage promotions.");
    }

    private void EnsurePromotionExists(int id)
    {
        if (!_promotionRepository.Exists(id)) throw new ArgumentException("Promotion not found.");
    }

    public Promotion GetPromotion(int id)
    {
        EnsurePromotionExists(id);
        return _promotionRepository.Get(id);
    }

    public void AddPromotion(Promotion promotion, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        _promotionRepository.Add(promotion);
    }

    public void DeletePromotion(int id, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsurePromotionExists(id);
        EnsureThereAreNoDepositsForThisPromotion(id);
        _promotionRepository.Delete(id);
    }

    private void EnsureThereAreNoDepositsForThisPromotion(int id)
    {
        if (_depositRepository.GetAll().Any(d => d.HasPromotion(id)))
            throw new ArgumentException("There are existing deposits for this promotion.");
    }

    public void ModifyPromotion(int id, Promotion newPromotion, Credentials credentials)
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