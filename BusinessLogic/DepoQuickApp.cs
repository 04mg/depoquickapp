using BusinessLogic.DTOs;

namespace BusinessLogic;

public class DepoQuickApp
{
    private readonly AuthManager _authManager = new();
    private readonly PromotionManager _promotionManager = new();
    private readonly DepositManager _depositManager = new();
    private readonly BookingManager _bookingManager = new();

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

    public AddPromotionDto GetPromotion(int id)
    {
        var promotion = _promotionManager.GetPromotionById(id);
        return new AddPromotionDto()
        {
            Label = promotion.Label,
            Discount = promotion.Discount,
            DateFrom = promotion.Validity.Item1,
            DateTo = promotion.Validity.Item2
        };
    }

    public void DeletePromotion(int i, Credentials credentials)
    {
        _depositManager.EnsureThereAreNoDepositsWithThisPromotion(i, credentials);
        _promotionManager.Delete(i, credentials);
    }

    public List<PromotionDto> ListAllPromotions(Credentials credentials)
    {
        return _promotionManager.Promotions.Select(p => new PromotionDto()
        {
            Id = p.Id,
            Label = p.Label,
            Discount = p.Discount,
            DateFrom = p.Validity.Item1,
            DateTo = p.Validity.Item2
        }).ToList();
    }

    public void ModifyPromotion(int id, PromotionDto promotionDto, Credentials credentials)
    {
        var promotion = new Promotion(
            id,
            promotionDto.Label,
            promotionDto.Discount,
            promotionDto.DateFrom,
            promotionDto.DateTo);
        _promotionManager.Modify(id, promotion, credentials);
    }

    public void AddDeposit(AddDepositDto depositDto, Credentials credentials)
    {
        var promotions = CreatePromotionListFromDto(depositDto);
        var deposit = new Deposit(1, depositDto.Area, depositDto.Size, depositDto.ClimateControl, promotions);
        _depositManager.Add(deposit, credentials);
    }

    public void DeleteDeposit(int id, Credentials credentials)
    {
        _bookingManager.EnsureThereAreNoBookingsWithThisDeposit(id);
        _depositManager.Delete(id, credentials);
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

    public List<DepositDto> ListAllDeposits(Credentials credentials)
    {
        return _depositManager.GetAllDeposits(credentials).Select(d => new DepositDto()
        {
            Id = d.Id,
            Area = d.Area,
            Size = d.Size,
            ClimateControl = d.ClimateControl,
            PromotionList = d.Promotions.Select(p => p.Id).ToList()
        }).ToList();
    }

    public DepositDto GetDeposit(int id, Credentials credentials)
    {
        var deposit = _depositManager.GetDepositById(id);
        return new DepositDto()
        {
            Id = deposit.Id,
            Area = deposit.Area,
            Size = deposit.Size,
            ClimateControl = deposit.ClimateControl,
            PromotionList = deposit.Promotions.Select(p => p.Id).ToList()
        };
    }

    public void AddBooking(AddBookingDto addBookingDto, Credentials credentials)
    {
        _depositManager.EnsureDepositExists(addBookingDto.DepositId, credentials);
        var deposit = _depositManager.GetAllDeposits(credentials).First(d => d.Id == addBookingDto.DepositId);
        var user = _authManager.GetUserByEmail(addBookingDto.Email, credentials);
        var priceCalculator = new PriceCalculator();
        var booking = new Booking(1, deposit, user, addBookingDto.DateFrom, addBookingDto.DateTo, priceCalculator);
        _bookingManager.Add(booking);
    }

    public double CalculateBookingPrice(AddBookingDto addBookingDto, Credentials credentials)
    {
        var priceCalculator = new PriceCalculator();
        var deposit = _depositManager.GetAllDeposits(credentials).First(d => d.Id == addBookingDto.DepositId);
        return priceCalculator.CalculatePrice(deposit,
            new Tuple<DateOnly, DateOnly>(addBookingDto.DateFrom, addBookingDto.DateTo));
    }

    public List<BookingDto> ListAllBookings(Credentials credentials)
    {
        return _bookingManager.GetAllBookings(credentials).Select(b => new BookingDto()
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

    public void ApproveBooking(int id, Credentials credentials)
    {
        _bookingManager.Approve(id, credentials);
    }

    public void RejectBooking(int id, string message, Credentials credentials)
    {
        _bookingManager.Reject(id, credentials, message);
    }
}