using BusinessLogic.DTOs;

namespace BusinessLogic;

public class DepoQuickApp
{
    private AuthManager _authManager;
    private PromotionManager _promotionManager;
    private DepositManager _depositManager;

    public DepoQuickApp()
    {
        _authManager = new AuthManager();
        _promotionManager = new PromotionManager();
        _depositManager = new DepositManager();
    }
    
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

    public Promotion GetPromotion(int id)
    {
        return _promotionManager.GetPromotionById(id);
    }

    public void DeletePromotion(int i, Credentials credentials)
    {
        _promotionManager.Delete(i, credentials);
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
        _depositManager.Delete(id, credentials);
    }

    private List<Promotion> CreatePromotionListFromDto(AddDepositDto depositDto)
    {
        var promotions = new List<Promotion>();
        foreach (var promotion in depositDto.PromotionList)
        {
            promotions.Add(_promotionManager.GetPromotionById(promotion));
        }
        return promotions;
    }

    public List<ListDepositDto> ListAllDeposits(Credentials credentials)
    {
        return _depositManager.Deposits.Select(d => new ListDepositDto()
        {
            Id = d.Id,
            Area = d.Area,
            Size = d.Size,
            ClimateControl = d.ClimateControl,
            PromotionList = d.Promotions.Select(p => p.Id).ToList()
        }).ToList();
    }
}


