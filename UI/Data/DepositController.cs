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
        return _app.ListAllDeposits();
    }

    public DepositDto GetDeposit(string name, Credentials credentials)
    {
        return _app.GetDeposit(name);
    }

    public void AddDeposit(AddDepositDto deposit, Credentials credentials)
    {
        _app.AddDeposit(deposit, credentials);
    }

    public void DeleteDeposit(string name, Credentials credentials)
    {
        _app.DeleteDeposit(name, credentials);
    }
}