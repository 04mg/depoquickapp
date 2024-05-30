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
        _depositService.AddDeposit(depositDto, credentials);
    }

    public void DeleteDeposit(string depositName, Credentials credentials)
    {
        _depositService.DeleteDeposit(depositName, credentials);
    }

    public List<DepositDto> ListAllDeposits()
    {
        return _depositService.GetAllDeposits().ToList();
    }

    public DepositDto GetDeposit(string depositName)
    {
        return _depositService.GetDeposit(depositName);
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

    public BookingDto GetBooking(int bookingId, Credentials credentials)
    {
        return _bookingService.GetBooking(bookingId, credentials);
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
        _depositService.AddAvailabilityPeriod(name, dateRange, credentials);
    }

    public double CalculateDepositPrice(PriceDto priceDto)
    {
        return _depositService.CalculateDepositPrice(priceDto);
    }
}