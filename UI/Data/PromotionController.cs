using BusinessLogic;

namespace UI.Data;

public class PromotionController
{
    private readonly DepoQuickApp _app;

    public PromotionController(DepoQuickApp app)
    {
        _app = app;
    }

    public AddPromotionDto GetPromotion(int id) => _app.GetPromotion(id);
}