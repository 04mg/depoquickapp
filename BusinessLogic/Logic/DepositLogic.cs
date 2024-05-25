using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Repositories;

namespace BusinessLogic.Logic;

public class DepositLogic
{
    private readonly IDepositRepository _depositRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPromotionRepository _promotionRepository;
    private List<Deposit> AllDeposits => _depositRepository.GetAll().ToList();

    public DepositLogic(IDepositRepository depositRepository, IBookingRepository bookingRepository,
        IPromotionRepository promotionRepository)
    {
        _depositRepository = depositRepository;
        _bookingRepository = bookingRepository;
        _promotionRepository = promotionRepository;
    }

    public void EnsureDepositExists(string name)
    {
        if (!_depositRepository.Exists(name)) throw new ArgumentException("Deposit not found.");
    }

    private static void EnsureUserIsAdmin(Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
            throw new UnauthorizedAccessException("Only administrators can manage deposits.");
    }

    public Deposit GetDeposit(string name)
    {
        return _depositRepository.Get(name);
    }

    public void Add(Deposit deposit, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsureDepositNameIsNotTaken(deposit.Name);
        _depositRepository.Add(deposit);
    }

    private void EnsureDepositNameIsNotTaken(string depositName)
    {
        depositName = depositName.ToLower();
        if (AllDeposits.Any(d => d.Name.ToLower() == depositName))
            throw new ArgumentException("Deposit name is already taken.");
    }

    public void Delete(string name, Credentials credentials)
    {
        EnsureDepositExists(name);
        EnsureUserIsAdmin(credentials);
        _depositRepository.Delete(name);
    }

    public IEnumerable<Deposit> GetAllDeposits()
    {
        return AllDeposits;
    }

    public void EnsureThereAreNoDepositsWithThisPromotion(int promotionId)
    {
        if (AllDeposits.Any(d => d.HasPromotion(promotionId)))
            throw new ArgumentException("There are existing deposits for this promotion.");
    }
}