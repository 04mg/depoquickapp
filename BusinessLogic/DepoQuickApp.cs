using BusinessLogic.Calculators;
using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Logic;
using BusinessLogic.Repositories;

namespace BusinessLogic;

public class DepoQuickApp
{
    private readonly AuthLogic _authLogic;
    private readonly BookingLogic _bookingLogic;
    private readonly DepositLogic _depositLogic;
    private readonly PromotionLogic _promotionLogic;

    public DepoQuickApp(IUserRepository userRepository, IPromotionRepository promotionRepository,
        IDepositRepository depositRepository, IBookingRepository bookingRepository)
    {
        _authLogic = new AuthLogic(userRepository);
        _promotionLogic = new PromotionLogic(promotionRepository, depositRepository);
        _depositLogic = new DepositLogic(depositRepository, bookingRepository);
        _bookingLogic = new BookingLogic(bookingRepository, userRepository, depositRepository);
    }

    public void RegisterUser(RegisterDto registerDto)
    {
        var user = new User(
            registerDto.NameSurname,
            registerDto.Email,
            registerDto.Password,
            registerDto.Rank);
        _authLogic.Register(user, registerDto.PasswordConfirmation);
    }

    public Credentials Login(LoginDto loginDto)
    {
        return _authLogic.Login(loginDto);
    }

    public void AddPromotion(AddPromotionDto addPromotionDto, Credentials credentials)
    {
        var promotion = new Promotion(
            1,
            addPromotionDto.Label,
            addPromotionDto.Discount,
            addPromotionDto.DateFrom,
            addPromotionDto.DateTo);
        _promotionLogic.Add(promotion, credentials);
    }

    public AddPromotionDto GetPromotion(int promotionId)
    {
        var promotion = _promotionLogic.GetPromotion(promotionId);
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
        _promotionLogic.Delete(promotionId, credentials);
    }

    public List<PromotionDto> ListAllPromotions(Credentials credentials)
    {
        return _promotionLogic.GetAllPromotions(credentials).Select(p => new PromotionDto
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
        _promotionLogic.Modify(promotionId, promotion, credentials);
    }

    public void AddDeposit(AddDepositDto depositDto, Credentials credentials)
    {
        var promotions = CreatePromotionListFromDto(depositDto);
        var deposit = new Deposit(depositDto.Name, depositDto.Area, depositDto.Size, depositDto.ClimateControl,
            promotions);
        _depositLogic.Add(deposit, credentials);
    }

    public void DeleteDeposit(string depositName, Credentials credentials)
    {
        _depositLogic.Delete(depositName, credentials);
    }

    private List<Promotion> CreatePromotionListFromDto(AddDepositDto depositDto)
    {
        return depositDto.PromotionList.Select(promotion => _promotionLogic.GetPromotion(promotion)).ToList();
    }

    public List<DepositDto> ListAllDeposits()
    {
        return _depositLogic.GetAllDeposits().Select(d => new DepositDto
        {
            Name = d.Name,
            Area = d.Area,
            Size = d.Size,
            ClimateControl = d.ClimateControl,
            PromotionList = d.Promotions.Select(p => p.Id).ToList()
        }).ToList();
    }

    public DepositDto GetDeposit(string depositName)
    {
        var deposit = _depositLogic.GetDeposit(depositName);
        return new DepositDto
        {
            Name = deposit.Name,
            Area = deposit.Area,
            Size = deposit.Size,
            ClimateControl = deposit.ClimateControl,
            PromotionList = deposit.Promotions.Select(p => p.Id).ToList()
        };
    }

    public void AddBooking(AddBookingDto addBookingDto, Credentials credentials)
    {
        var deposit = _depositLogic.GetDeposit(addBookingDto.DepositName);
        var user = _authLogic.GetUser(addBookingDto.Email, credentials);
        var priceCalculator = new PriceCalculator();
        var booking = new Booking(1, deposit, user, addBookingDto.DateFrom, addBookingDto.DateTo, priceCalculator);
        _bookingLogic.AddBooking(booking);
    }

    public double CalculateBookingPrice(AddBookingDto addBookingDto)
    {
        var priceCalculator = new PriceCalculator();
        var deposit = _depositLogic.GetDeposit(addBookingDto.DepositName);
        return priceCalculator.CalculatePrice(deposit,
            new Tuple<DateOnly, DateOnly>(addBookingDto.DateFrom, addBookingDto.DateTo));
    }

    public List<BookingDto> ListAllBookings(Credentials credentials)
    {
        return _bookingLogic.GetAllBookings(credentials).Select(b => new BookingDto
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
        return _bookingLogic.GetBookingsByEmail(email, credentials).Select(b => new BookingDto
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
        var booking = _bookingLogic.GetBooking(bookingId);
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
        _bookingLogic.ApproveBooking(bookingId, credentials);
    }

    public void RejectBooking(int bookingId, string message, Credentials credentials)
    {
        _bookingLogic.RejectBooking(bookingId, credentials, message);
    }
}