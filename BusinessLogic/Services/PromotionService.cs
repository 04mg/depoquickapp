using BusinessLogic.DTOs;
using DataAccess.Repositories;
using Domain;

namespace BusinessLogic.Services;

public class PromotionService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IDepositRepository _depositRepository;

    public PromotionService(IPromotionRepository promotionRepository, IDepositRepository depositRepository)
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

    public PromotionDto GetPromotion(int id)
    {
        EnsurePromotionExists(id);
        return PromotionToDto(_promotionRepository.Get(id));
    }

    private static PromotionDto PromotionToDto(Promotion promotion)
    {
        return new PromotionDto
        {
            Id = promotion.Id,
            Label = promotion.Label,
            Discount = promotion.Discount,
            DateFrom = promotion.Validity.StartDate,
            DateTo = promotion.Validity.EndDate
        };
    }

    public void AddPromotion(PromotionDto promotionDto, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        _promotionRepository.Add(PromotionFromDto(promotionDto));
    }

    private static Promotion PromotionFromDto(PromotionDto promotionDto)
    {
        return new Promotion(promotionDto.Id, promotionDto.Label, promotionDto.Discount, promotionDto.DateFrom,
            promotionDto.DateTo);
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

    public void ModifyPromotion(PromotionDto newPromotion, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsurePromotionExists(newPromotion.Id);
        _promotionRepository.Update(PromotionFromDto(newPromotion));
    }

    public IEnumerable<PromotionDto> GetAllPromotions(Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        return PromotionsToDtoList(_promotionRepository.GetAll());
    }

    private static IEnumerable<PromotionDto> PromotionsToDtoList(IEnumerable<Promotion> promotions)
    {
        return promotions.Select(PromotionToDto);
    }
}