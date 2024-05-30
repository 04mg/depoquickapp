using BusinessLogic.Calculators;
using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Repositories;

namespace BusinessLogic.Services;

public class DepositService
{
    private readonly IDepositRepository _depositRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPromotionRepository _promotionRepository;
    private List<Deposit> AllDeposits => _depositRepository.GetAll().ToList();

    public DepositService(IDepositRepository depositRepository, IBookingRepository bookingRepository,
        IPromotionRepository promotionRepository)
    {
        _depositRepository = depositRepository;
        _bookingRepository = bookingRepository;
        _promotionRepository = promotionRepository;
    }

    private void EnsureDepositExists(string name)
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
        EnsureDepositExists(name);
        return _depositRepository.Get(name);
    }

    public void AddDeposit(Deposit deposit, Credentials credentials)
    {
        EnsureAllPromotionsExist(deposit.Promotions);
        EnsureUserIsAdmin(credentials);
        EnsureDepositNameIsNotTaken(deposit.Name);
        _depositRepository.Add(deposit);
    }

    private void EnsureAllPromotionsExist(IEnumerable<Promotion> promotions)
    {
        if (promotions.Any(p => !_promotionRepository.Exists(p.Id)))
        {
            throw new ArgumentException("Promotion not found.");
        }
    }

    private void EnsureDepositNameIsNotTaken(string depositName)
    {
        depositName = depositName.ToLower();
        if (AllDeposits.Any(d => d.Name.ToLower() == depositName))
            throw new ArgumentException("Deposit name is already taken.");
    }

    public void DeleteDeposit(string name, Credentials credentials)
    {
        EnsureThereAreNoBookingsForThisDeposit(name);
        EnsureDepositExists(name);
        EnsureUserIsAdmin(credentials);
        _depositRepository.Delete(name);
    }

    public IEnumerable<Deposit> GetAllDeposits()
    {
        return AllDeposits;
    }

    private void EnsureThereAreNoBookingsForThisDeposit(string depositName)
    {
        if (_bookingRepository.GetAll().Any(b => b.Deposit.Name == depositName))
            throw new ArgumentException("There are existing bookings for this deposit.");
    }

    public void AddAvailabilityPeriod(string deposit, DateRange availabilityPeriod, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsureDepositExists(deposit);
        GetDeposit(deposit).AddAvailabilityPeriod(availabilityPeriod);
    }

    public double CalculateDepositPrice(PriceDto priceDto)
    {
        EnsureDepositExists(priceDto.Deposit.Name);
        var deposit = _depositRepository.Get(priceDto.Deposit.Name);
        return new PriceCalculator().CalculatePrice(deposit, priceDto.DateFrom, priceDto.DateTo);
    }
}