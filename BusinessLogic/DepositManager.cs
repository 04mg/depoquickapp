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
        var deposit = new Deposit(NextDepositId, addDepositDto.Area, addDepositDto.Size, addDepositDto.ClimateControl,
            addDepositDto.PromotionList, promotionManager);
        Deposits.Add(deposit);
    }

    public void Delete(int id, Credentials credentials)
    {
        var deposit = Deposits.FirstOrDefault(d => d.Id == id);
        if (deposit == null)
        {
            return;
        }

        Deposits.Remove(deposit);
    }
    
    private int NextDepositId => Deposits.Count > 0 ? Deposits.Max(d => d.Id) + 1 : 1;
}