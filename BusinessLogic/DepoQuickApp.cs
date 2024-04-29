namespace BusinessLogic;

public class DepoQuickApp
{
    private AuthManager _authManager;
    private PromotionManager _promotionManager;
    public DepoQuickApp()
    {
        _authManager = new AuthManager();
        _promotionManager = new PromotionManager();
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
}


