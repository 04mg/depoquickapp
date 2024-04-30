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

    public List<ListDepositDto> ListAllDeposits(Credentials credentials) => _app.ListAllDeposits(credentials);
}
