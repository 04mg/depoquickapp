namespace BusinessLogic;

public class DepositManager
{
    public DepositManager()
    {
        Deposits = new List<Deposit>();
    }

    public List<Deposit> Deposits { get; set; }

    public void Add(AddDepositDto addDepositDto, Credentials credentials, PromotionManager promotionManager)
    {
        var deposit = new Deposit(addDepositDto.Area, addDepositDto.Size, addDepositDto.ClimateControl,
            addDepositDto.PromotionList, promotionManager);
        Deposits.Add(deposit);
    }
}