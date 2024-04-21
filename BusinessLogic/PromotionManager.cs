namespace BusinessLogic;

public class PromotionManager
{
    public List<Promotion> Promotions { get; private set; }
    
    public PromotionManager()
    {
        Promotions = new List<Promotion>();
    }
    
    public void Add(PromotionModel promotionModel)
    {
        Promotions.Add(new Promotion(promotionModel));
    }
}