using BusinessLogic;
using BusinessLogic.DTOs;

namespace UI.Data;

public class PromotionController
{
    private readonly DepoQuickApp _app;

    public PromotionController(DepoQuickApp app)
    {
        _app = app;
    }

    public AddPromotionDto GetPromotion(int id)
    {
        return _app.GetPromotion(id);
    }

    public List<PromotionDto> ListAllPromotions(Credentials credentials)
    {
        return _app.ListAllPromotions(credentials);
    }

    public void AddPromotion(AddPromotionDto dto, Credentials credentials)
    {
        _app.AddPromotion(dto, credentials);
    }

    public void ModifyPromotion(int id, PromotionDto dto, Credentials credentials)
    {
        _app.ModifyPromotion(id, dto, credentials);
    }

    public void DeletePromotion(int id, Credentials credentials)
    {
        _app.DeletePromotion(id, credentials);
    }
}