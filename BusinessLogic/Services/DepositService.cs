using BusinessLogic.DTOs;
using BusinessLogic.Exceptions;
using DataAccess.Interfaces;
using Domain;
using Domain.Enums;

namespace BusinessLogic.Services;

public class DepositService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IDepositRepository _depositRepository;
    private readonly IPromotionRepository _promotionRepository;

    public DepositService(IDepositRepository depositRepository, IBookingRepository bookingRepository,
        IPromotionRepository promotionRepository)
    {
        _depositRepository = depositRepository;
        _bookingRepository = bookingRepository;
        _promotionRepository = promotionRepository;
    }

    private IEnumerable<Deposit> AllDeposits => _depositRepository.GetAll();

    private void EnsureDepositExists(string name)
    {
        if (!_depositRepository.Exists(name)) throw new BusinessLogicException("Deposit not found.");
    }

    private static void EnsureUserIsAdmin(Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
            throw new BusinessLogicException("Only administrators can manage deposits.");
    }

    public DepositDto GetDeposit(string depositName)
    {
        EnsureDepositExists(depositName);
        return DepositToDto(_depositRepository.Get(depositName));
    }

    private static DepositDto DepositToDto(Deposit deposit)
    {
        return new DepositDto
        {
            Name = deposit.Name,
            Area = deposit.Area.ToString(),
            Size = deposit.Size.ToString(),
            ClimateControl = deposit.ClimateControl,
            Promotions = PromotionsToDto(deposit.Promotions),
            AvailabilityPeriods = DateRangeToDto(deposit.GetAvailablePeriods())
        };
    }

    private static List<DateRangeDto> DateRangeToDto(IEnumerable<DateRange.DateRange> dateRanges)
    {
        return dateRanges.Select(dr => new DateRangeDto { StartDate = dr.StartDate, EndDate = dr.EndDate }).ToList();
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

    private static List<PromotionDto> PromotionsToDto(IEnumerable<Promotion> promotions)
    {
        return promotions.Select(PromotionToDto).ToList();
    }

    public void AddDeposit(DepositDto depositDto, Credentials credentials)
    {
        var deposit = DepositFromDto(depositDto);
        EnsureAllPromotionsExist(deposit.Promotions);
        EnsureUserIsAdmin(credentials);
        EnsureDepositNameIsNotTaken(deposit.Name);
        _depositRepository.Add(deposit);
    }

    private static Deposit DepositFromDto(DepositDto depositDto)
    {
        return new Deposit(depositDto.Name, DepositAreaFromDto(depositDto.Area), DepositSizeFromDto(depositDto.Size),
            depositDto.ClimateControl,
            PromotionsFromDto(depositDto.Promotions));
    }

    private static DepositArea DepositAreaFromDto(string areaString)
    {
        if (!Enum.TryParse(areaString, out DepositArea areaEnum))
            throw new BusinessLogicException("Invalid area.");
        return areaEnum;
    }

    private static DepositSize DepositSizeFromDto(string sizeString)
    {
        if (!Enum.TryParse(sizeString, out DepositSize sizeEnum))
            throw new BusinessLogicException("Invalid size.");
        return sizeEnum;
    }

    private static Promotion PromotionFromDto(PromotionDto promotionDto)
    {
        return new Promotion(promotionDto.Id, promotionDto.Label, promotionDto.Discount, promotionDto.DateFrom,
            promotionDto.DateTo);
    }

    private static List<Promotion> PromotionsFromDto(IEnumerable<PromotionDto> promotionsDto)
    {
        return promotionsDto.Select(PromotionFromDto).ToList();
    }

    private void EnsureAllPromotionsExist(IEnumerable<Promotion> promotions)
    {
        if (promotions.Any(p => !_promotionRepository.Exists(p.Id)))
            throw new BusinessLogicException("Promotion not found.");
    }

    private void EnsureDepositNameIsNotTaken(string depositName)
    {
        depositName = depositName.ToLower();
        if (AllDeposits.Any(d => d.Name.ToLower() == depositName))
            throw new BusinessLogicException("Deposit name is already taken.");
    }

    public void DeleteDeposit(string name, Credentials credentials)
    {
        EnsureThereAreNoBookingsForThisDeposit(name);
        EnsureDepositExists(name);
        EnsureUserIsAdmin(credentials);
        _depositRepository.Delete(name);
    }

    public IEnumerable<DepositDto> GetAllDeposits()
    {
        return AllDeposits.Select(DepositToDto);
    }

    private void EnsureThereAreNoBookingsForThisDeposit(string depositName)
    {
        if (_bookingRepository.GetAll().Any(b => b.Deposit.Name == depositName))
            throw new BusinessLogicException("There are existing bookings for this deposit.");
    }

    public void AddAvailabilityPeriod(string depositName, DateRangeDto dateRange, Credentials credentials)
    {
        EnsureUserIsAdmin(credentials);
        EnsureDepositExists(depositName);
        var deposit = _depositRepository.Get(depositName);
        deposit.AddAvailabilityPeriod(DateRangeFromDto(dateRange));
        _depositRepository.Update(deposit);
    }

    private static DateRange.DateRange DateRangeFromDto(DateRangeDto dateRangeDto)
    {
        return new DateRange.DateRange(dateRangeDto.StartDate, dateRangeDto.EndDate);
    }

    private static void EnsureDateRangeIsValid(DateRangeDto dateRangeDto)
    {
        if (dateRangeDto.StartDate < DateOnly.FromDateTime(DateTime.Now) ||
            dateRangeDto.EndDate < DateOnly.FromDateTime(DateTime.Now))
            throw new BusinessLogicException("Date range cannot be in the past.");
    }

    public IEnumerable<DepositDto> GetDepositsByAvailabilityPeriod(DateRangeDto dateRangeDto)
    {
        EnsureDateRangeIsValid(dateRangeDto);
        var dateRange = DateRangeFromDto(dateRangeDto);
        return AllDeposits.Where(d => d.IsAvailable(dateRange)).Select(DepositToDto);
    }
}