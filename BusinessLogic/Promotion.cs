namespace BusinessLogic;

public class Promotion
{
    private string _label;
    private int _discount;
    private DateOnly _from;
    private DateOnly _to;

    public string Label
    {
        get => _label;
        set
        {
            //Use IsLetterOrDigit
            if (!value.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
            {
                throw new ArgumentException("Label format is invalid, it can't contain symbols");
            }
            if (value.Length > 20)
            {
                throw new ArgumentException("Label format is invalid, length must be lesser or equal than 20");
            }

            _label = value;
        }
    }
    
    public int Discount
    {
        get => _discount;
        set
        {
            if (value < 5 || value > 70)
            {
                throw new ArgumentException("Invalid discount, it must be between 5% and 70%");
            }

            _discount = value;
        }
    }
    public Promotion(string label, int discount, DateOnly from, DateOnly to)
    {
        Label = label;
        Discount = discount;
        _from = from;
        _to = to;
    }
    
    
}