using BusinessLogic.Calculators;
using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Repositories;
using BusinessLogic.Services;

namespace BusinessLogic;

public class DepoQuickApp
{
    private readonly UserService _userService;
    private readonly BookingService _bookingService;
    private readonly DepositService _depositService;
    private readonly PromotionService _promotionService;

    public DepoQuickApp(IUserRepository userRepository, IPromotionRepository promotionRepository,
        IDepositRepository depositRepository, IBookingRepository bookingRepository)
    {
        _userService = new UserService(userRepository);
        _promotionService = new PromotionService(promotionRepository, depositRepository);
        _depositService = new DepositService(depositRepository, bookingRepository, promotionRepository);
        _bookingService = new BookingService(bookingRepository, depositRepository, userRepository);
    }

    public void RegisterUser(RegisterDto registerDto)
    {
        _userService.Register(registerDto);
    }

    public Credentials Login(LoginDto loginDto)
    {
        return _userService.Login(loginDto);
    }

    public void AddPromotion(PromotionDto promotionDto, Credentials credentials)
    {
        _promotionService.AddPromotion(promotionDto, credentials);
    }

    public PromotionDto GetPromotion(int promotionId)
    {
        return _promotionService.GetPromotion(promotionId);
    }

    public void DeletePromotion(int promotionId, Credentials credentials)
    {
        _promotionService.DeletePromotion(promotionId, credentials);
    }

    public IEnumerable<PromotionDto> ListAllPromotions(Credentials credentials)
    {
        return _promotionService.GetAllPromotions(credentials);
    }

    public void ModifyPromotion(int promotionId, PromotionDto promotionDto, Credentials credentials)
    {
        _promotionService.ModifyPromotion(promotionId, promotionDto, credentials);
    }

    public void AddDeposit(DepositDto depositDto, Credentials credentials)
    {
        var promotions = CreatePromotionListFromDto(depositDto);
        var deposit = new Deposit(depositDto.Name, depositDto.Area, depositDto.Size, depositDto.ClimateControl,
            promotions);
        _depositService.AddDeposit(deposit, credentials);
    }

    public void DeleteDeposit(string depositName, Credentials credentials)
    {
        _depositService.DeleteDeposit(depositName, credentials);
    }

    private List<Promotion> CreatePromotionListFromDto(DepositDto depositDto)
    {
        return depositDto.PromotionList.Select(p =>
        {
            var dto = _promotionService.GetPromotion(p);
            return new Promotion(dto.Id, dto.Label, dto.Discount, dto.DateFrom, dto.DateTo);
        }).ToList();
    }

    public List<DepositDto> ListAllDeposits()
    {
        return _depositService.GetAllDeposits().Select(d => new DepositDto
        {
            Name = d.Name,
            Area = d.Area,
            Size = d.Size,
            ClimateControl = d.ClimateControl,
            PromotionList = d.Promotions.Select(p => p.Id).ToList(),
            AvailabilityPeriods = d.GetAvailablePeriods().Select(p => new DateRangeDto
            {
                StartDate = p.StartDate,
                EndDate = p.EndDate
            }).ToList()
        }).ToList();
    }

    public DepositDto GetDeposit(string depositName)
    {
        var deposit = _depositService.GetDeposit(depositName);
        return new DepositDto
        {
            Name = deposit.Name,
            Area = deposit.Area,
            Size = deposit.Size,
            ClimateControl = deposit.ClimateControl,
            PromotionList = deposit.Promotions.Select(p => p.Id).ToList(),
            AvailabilityPeriods = deposit.GetAvailablePeriods().Select(p => new DateRangeDto
            {
                StartDate = p.StartDate,
                EndDate = p.EndDate
            }).ToList()
        };
    }

    public void AddBooking(BookingDto bookingDto, Credentials credentials)
    {
        _bookingService.AddBooking(bookingDto, credentials);
    }

    public List<BookingDto> ListAllBookings(Credentials credentials)
    {
        return _bookingService.GetAllBookings(credentials).ToList();
    }

    public List<BookingDto> ListAllBookingsByEmail(string email, Credentials credentials)
    {
        return _bookingService.GetBookingsByEmail(email, credentials).ToList();
    }

    public BookingDto GetBooking(int bookingId)
    {
        return _bookingService.GetBooking(bookingId);
    }

    public void ApproveBooking(int bookingId, Credentials credentials)
    {
        _bookingService.ApproveBooking(bookingId, credentials);
    }

    public void RejectBooking(int bookingId, string message, Credentials credentials)
    {
        _bookingService.RejectBooking(bookingId, credentials, message);
    }

    public void AddAvailabilityPeriod(string name, DateRangeDto dateRange, Credentials credentials)
    {
        var period = new DateRange(dateRange.StartDate, dateRange.EndDate);
        _depositService.AddAvailabilityPeriod(name, period, credentials);
    }

    public double CalculateDepositPrice(PriceDto priceDto)
    {
        return _depositService.CalculateDepositPrice(priceDto);
    }
}