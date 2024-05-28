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
        var user = new User(
            registerDto.NameSurname,
            registerDto.Email,
            registerDto.Password,
            registerDto.Rank);
        _userService.Register(user, registerDto.PasswordConfirmation);
    }

    public Credentials Login(LoginDto loginDto)
    {
        return _userService.Login(loginDto.Email, loginDto.Password);
    }

    public void AddPromotion(AddPromotionDto addPromotionDto, Credentials credentials)
    {
        var promotion = new Promotion(
            1,
            addPromotionDto.Label,
            addPromotionDto.Discount,
            addPromotionDto.DateFrom,
            addPromotionDto.DateTo);
        _promotionService.AddPromotion(promotion, credentials);
    }

    public AddPromotionDto GetPromotion(int promotionId)
    {
        var promotion = _promotionService.GetPromotion(promotionId);
        return new AddPromotionDto
        {
            Label = promotion.Label,
            Discount = promotion.Discount,
            DateFrom = promotion.Validity.Item1,
            DateTo = promotion.Validity.Item2
        };
    }

    public void DeletePromotion(int promotionId, Credentials credentials)
    {
        _promotionService.DeletePromotion(promotionId, credentials);
    }

    public List<PromotionDto> ListAllPromotions(Credentials credentials)
    {
        return _promotionService.GetAllPromotions(credentials).Select(p => new PromotionDto
        {
            Id = p.Id,
            Label = p.Label,
            Discount = p.Discount,
            DateFrom = p.Validity.Item1,
            DateTo = p.Validity.Item2
        }).ToList();
    }

    public void ModifyPromotion(int promotionId, PromotionDto promotionDto, Credentials credentials)
    {
        var promotion = new Promotion(
            promotionId,
            promotionDto.Label,
            promotionDto.Discount,
            promotionDto.DateFrom,
            promotionDto.DateTo);
        _promotionService.ModifyPromotion(promotionId, promotion, credentials);
    }

    public void AddDeposit(AddDepositDto depositDto, Credentials credentials)
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

    private List<Promotion> CreatePromotionListFromDto(AddDepositDto depositDto)
    {
        return depositDto.PromotionList.Select(promotion => _promotionService.GetPromotion(promotion)).ToList();
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

    public void AddBooking(AddBookingDto addBookingDto, Credentials credentials)
    {
        var deposit = _depositService.GetDeposit(addBookingDto.DepositName);
        var user = _userService.GetUser(addBookingDto.Email, credentials);
        var priceCalculator = new PriceCalculator();
        var booking = new Booking(1, deposit, user, addBookingDto.DateFrom, addBookingDto.DateTo, priceCalculator);
        _bookingService.AddBooking(booking);
    }

    public double CalculateBookingPrice(AddBookingDto addBookingDto)
    {
        var priceCalculator = new PriceCalculator();
        var deposit = _depositService.GetDeposit(addBookingDto.DepositName);
        return priceCalculator.CalculatePrice(deposit,
            new Tuple<DateOnly, DateOnly>(addBookingDto.DateFrom, addBookingDto.DateTo));
    }

    public List<BookingDto> ListAllBookings(Credentials credentials)
    {
        return _bookingService.GetAllBookings(credentials).Select(b => new BookingDto
        {
            Id = b.Id,
            DepositName = b.Deposit.Name,
            Email = b.Client.Email,
            DateFrom = b.Duration.Item1,
            DateTo = b.Duration.Item2,
            Stage = b.Stage.ToString(),
            Message = b.Message
        }).ToList();
    }

    public List<BookingDto> ListAllBookingsByEmail(string email, Credentials credentials)
    {
        return _bookingService.GetBookingsByEmail(email, credentials).Select(b => new BookingDto
        {
            Id = b.Id,
            DepositName = b.Deposit.Name,
            Email = b.Client.Email,
            DateFrom = b.Duration.Item1,
            DateTo = b.Duration.Item2,
            Stage = b.Stage.ToString(),
            Message = b.Message
        }).ToList();
    }

    public BookingDto GetBooking(int bookingId)
    {
        var booking = _bookingService.GetBooking(bookingId);
        return new BookingDto
        {
            Id = booking.Id,
            DepositName = booking.Deposit.Name,
            Email = booking.Client.Email,
            DateFrom = booking.Duration.Item1,
            DateTo = booking.Duration.Item2,
            Stage = booking.Stage.ToString(),
            Message = booking.Message
        };
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
}