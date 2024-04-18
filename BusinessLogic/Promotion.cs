namespace BusinessLogic;

public class Promotion
{
    private string _label;
    private int _discount;
    private Tuple<DateOnly, DateOnly> _validity;

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
    
    public Tuple<DateOnly, DateOnly> Validity
    {
        get => _validity;
        set
        {
            if (value.Item1 > value.Item2)
            {
                throw new ArgumentException("DateFrom must be lesser than DateTo");
            }

            _validity = value;
        }
    }

    public Promotion(string label, int discount, DateOnly from, DateOnly to)
    {
        Label = label;
        _discount = discount;
        Validity = new Tuple<DateOnly, DateOnly>(from, to);
    }
}