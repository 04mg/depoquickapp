using BusinessLogic.Calculators;
using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Managers;

namespace BusinessLogic;

public class DepoQuickApp
{
    private readonly AuthManager _authManager = new();
    private readonly BookingManager _bookingManager = new();
    private readonly DepositManager _depositManager = new();
    private readonly PromotionManager _promotionManager = new();

    public void RegisterUser(RegisterDto registerDto)
    {
        var user = new User(
            registerDto.NameSurname,
            registerDto.Email,
            registerDto.Password,
            registerDto.Rank);
        _authManager.Register(user, registerDto.PasswordConfirmation);
    }

    public Credentials Login(LoginDto loginDto)
    {
        return _authManager.Login(loginDto);
    }

    public void AddPromotion(AddPromotionDto addPromotionDto, Credentials credentials)
    {
        var promotion = new Promotion(
            1,
            addPromotionDto.Label,
            addPromotionDto.Discount,
            addPromotionDto.DateFrom,
            addPromotionDto.DateTo);
        _promotionManager.Add(promotion, credentials);
    }

    public AddPromotionDto GetPromotion(int promotionId)
    {
        var promotion = _promotionManager.GetPromotionById(promotionId);
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
        _depositManager.EnsureThereAreNoDepositsWithThisPromotion(promotionId, credentials);
        _promotionManager.Delete(promotionId, credentials);
    }

    public List<PromotionDto> ListAllPromotions(Credentials credentials)
    {
        return _promotionManager.GetAllPromotions(credentials).Select(p => new PromotionDto
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
        _promotionManager.Modify(promotionId, promotion, credentials);
    }

    public void AddDeposit(AddDepositDto depositDto, Credentials credentials)
    {
        var promotions = CreatePromotionListFromDto(depositDto);
        var deposit = new Deposit(depositDto.Name, 1, depositDto.Area, depositDto.Size, depositDto.ClimateControl, promotions);
        _depositManager.Add(deposit, credentials);
    }

    public void DeleteDeposit(int depositId, Credentials credentials)
    {
        _bookingManager.EnsureThereAreNoBookingsWithThisDeposit(depositId);
        _depositManager.Delete(depositId, credentials);
    }

    private List<Promotion> CreatePromotionListFromDto(AddDepositDto depositDto)
    {
        var promotions = new List<Promotion>();
        foreach (var promotion in depositDto.PromotionList)
        {
            _promotionManager.EnsurePromotionExists(promotion);
            promotions.Add(_promotionManager.GetPromotionById(promotion));
        }

        return promotions;
    }

    public List<DepositDto> ListAllDeposits()
    {
        return _depositManager.GetAllDeposits().Select(d => new DepositDto
        {
            Name = d.Name,
            Id = d.Id,
            Area = d.Area,
            Size = d.Size,
            ClimateControl = d.ClimateControl,
            PromotionList = d.Promotions.Select(p => p.Id).ToList()
        }).ToList();
    }

    public DepositDto GetDeposit(int depositId)
    {
        var deposit = _depositManager.GetDepositById(depositId);
        return new DepositDto
        {
            Name = deposit.Name,
            Id = deposit.Id,
            Area = deposit.Area,
            Size = deposit.Size,
            ClimateControl = deposit.ClimateControl,
            PromotionList = deposit.Promotions.Select(p => p.Id).ToList()
        };
    }

    public void AddBooking(AddBookingDto addBookingDto, Credentials credentials)
    {
        _depositManager.EnsureDepositExists(addBookingDto.DepositId);
        var deposit = _depositManager.GetDepositById(addBookingDto.DepositId);
        var user = _authManager.GetUserByEmail(addBookingDto.Email, credentials);
        var priceCalculator = new PriceCalculator();
        var booking = new Booking(1, deposit, user, addBookingDto.DateFrom, addBookingDto.DateTo, priceCalculator);
        _bookingManager.Add(booking);
    }

    public double CalculateBookingPrice(AddBookingDto addBookingDto)
    {
        var priceCalculator = new PriceCalculator();
        var deposit = _depositManager.GetDepositById(addBookingDto.DepositId);
        return priceCalculator.CalculatePrice(deposit,
            new Tuple<DateOnly, DateOnly>(addBookingDto.DateFrom, addBookingDto.DateTo));
    }

    public List<BookingDto> ListAllBookings(Credentials credentials)
    {
        return _bookingManager.GetAllBookings(credentials).Select(b => new BookingDto
        {
            Id = b.Id,
            DepositId = b.Deposit.Id,
            Email = b.Client.Email,
            DateFrom = b.Duration.Item1,
            DateTo = b.Duration.Item2,
            Stage = b.Stage.ToString(),
            Message = b.Message
        }).ToList();
    }

    public List<BookingDto> ListAllBookingsByEmail(string email, Credentials credentials)
    {
        return _bookingManager.GetBookingsByEmail(email, credentials).Select(b => new BookingDto
        {
            Id = b.Id,
            DepositId = b.Deposit.Id,
            Email = b.Client.Email,
            DateFrom = b.Duration.Item1,
            DateTo = b.Duration.Item2,
            Stage = b.Stage.ToString(),
            Message = b.Message
        }).ToList();
    }

    public BookingDto GetBooking(int bookingId)
    {
        var booking = _bookingManager.GetBookingById(bookingId);
        return new BookingDto
        {
            Id = booking.Id,
            DepositId = booking.Deposit.Id,
            Email = booking.Client.Email,
            DateFrom = booking.Duration.Item1,
            DateTo = booking.Duration.Item2,
            Stage = booking.Stage.ToString(),
            Message = booking.Message
        };
    }

    public void ApproveBooking(int bookingId, Credentials credentials)
    {
        _bookingManager.Approve(bookingId, credentials);
    }

    public void RejectBooking(int bookingId, string message, Credentials credentials)
    {
        _bookingManager.Reject(bookingId, credentials, message);
    }
}