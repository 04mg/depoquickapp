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
        EnsureThereAreNoDepositsWithThisPromotion(i, credentials);
        _promotionManager.Delete(i, credentials);
    }

    private void EnsureThereAreNoDepositsWithThisPromotion(int i, Credentials credentials)
    {
        var deposits = ListAllDeposits(credentials);
        foreach (var deposit in deposits)
        {
            EnsureThatPromotionNotExists(i, deposit.PromotionList);
        }
    }

    private static void EnsureThatPromotionNotExists(int id, List<int> promotionList)
    {
        if (promotionList.Contains(id))
            throw new ArgumentException("Cant delete promotion, it is included in deposits.");
    }

    public List<ModifyPromotionDto> ListAllPromotions(Credentials credentials)
    {
        return _promotionManager.Promotions.Select(p => new ModifyPromotionDto()
        {
            Id = p.Id,
            Label = p.Label,
            Discount = p.Discount,
            DateFrom = p.Validity.Item1,
            DateTo = p.Validity.Item2
        }).ToList();
    }

    public void ModifyPromotion(int id, ModifyPromotionDto modifyPromotionDto, Credentials credentials)
    {
        var promotion = new Promotion(
            id,
            modifyPromotionDto.Label,
            modifyPromotionDto.Discount,
            modifyPromotionDto.DateFrom,
            modifyPromotionDto.DateTo);
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
            EnsurePromotionExists(promotion);
            promotions.Add(_promotionManager.GetPromotionById(promotion));
        }

        return promotions;
    }

    private void EnsurePromotionExists(int id)
    {
        if (!_promotionManager.Promotions.Any(p => p.Id == id))
        {
            throw new ArgumentException("Promotion not found.");
        }
    }

    public List<ListDepositDto> ListAllDeposits(Credentials credentials)
    {
        return _depositManager.GetAllDeposits(credentials).Select(d => new ListDepositDto()
        {
            Id = d.Id,
            Area = d.Area,
            Size = d.Size,
            ClimateControl = d.ClimateControl,
            PromotionList = d.Promotions.Select(p => p.Id).ToList()
        }).ToList();
    }

    public ListDepositDto GetDeposit(int id, Credentials credentials)
    {
        var deposit = _depositManager.GetDepositById(id);
        return new ListDepositDto()
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
        EnsureUserExists(addBookingDto.Email);
        EnsureDepositExists(addBookingDto.DepositId, credentials);
        var deposit = _depositManager.GetAllDeposits(credentials).First(d => d.Id == addBookingDto.DepositId);
        var user = _authManager.GetUserByEmail(addBookingDto.Email, credentials);
        var booking = new Booking(1, deposit, user, addBookingDto.DateFrom, addBookingDto.DateTo);
        _bookingManager.Add(booking);
    }

    private void EnsureDepositExists(int depositId, Credentials credentials)
    {
        if (!_depositManager.GetAllDeposits(credentials).Any(d => d.Id == depositId))
        {
            throw new ArgumentException("Deposit not found.");
        }
    }

    private void EnsureUserExists(string email)
    {
        if (!_authManager.Exists(email))
        {
            throw new ArgumentException("User not found.");
        }
    }

    public List<ListBookingDto> ListAllBookings(Credentials credentials)
    {
        return _bookingManager.GetAllBookings(credentials).Select(b => new ListBookingDto()
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