namespace BusinessLogic;

public class DepositManager
{
    public DepositManager()
    {
        Deposits = new List<Deposit>();
    }

    public List<Deposit> Deposits { get; set; }

    private void EnsureDepositExists(int id)
    {
        if (Deposits.All(d => d.Id != id))
        {
            throw new ArgumentException("Deposit not found");
        }
    }
    
    private Deposit GetDepositById(int id)
    {
        return Deposits.First(d => d.Id == id);
    }

    public void Add(AddDepositDto addDepositDto, Credentials credentials, PromotionManager promotionManager)
    {
        var deposit = new Deposit(NextDepositId, addDepositDto.Area, addDepositDto.Size, addDepositDto.ClimateControl,
            addDepositDto.PromotionList, promotionManager);
        Deposits.Add(deposit);
    }

    public void Delete(int id, Credentials credentials)
    {
        EnsureDepositExists(id);

        var deposit = GetDepositById(id);
        Deposits.Remove(deposit);
    }

    private int NextDepositId => Deposits.Count > 0 ? Deposits.Max(d => d.Id) + 1 : 1;
}