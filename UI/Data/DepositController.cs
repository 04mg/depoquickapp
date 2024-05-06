using BusinessLogic;
using BusinessLogic.DTOs;

namespace UI.Data;

public class DepositController
{
    private readonly DepoQuickApp _app;

    public DepositController(DepoQuickApp app)
    {
        _app = app;
    }

    public List<DepositDto> ListAllDeposits(Credentials credentials)
    {
        return _app.ListAllDeposits(credentials);
    }

    public DepositDto GetDeposit(int id, Credentials credentials) => _app.GetDeposit(id, credentials);
    
    public void AddDeposit(AddDepositDto deposit, Credentials credentials) => _app.AddDeposit(deposit, credentials);
    
    public void DeleteDeposit(int id, Credentials credentials) => _app.DeleteDeposit(id, credentials);
}