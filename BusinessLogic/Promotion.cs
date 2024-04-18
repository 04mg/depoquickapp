namespace BusinessLogic;

public class Promotion
{
    private string _label = "";
    private int _discount;
    private Tuple<DateOnly, DateOnly> _validity;

    public string Label
    {
        get => _label;
        set
        {
            EnsureLabelHasNoSymbols(value);
            EnsureLabelLengthIsLesserOrEqualThan20(value);
            _label = value;
        }
    }
    
    public Tuple<DateOnly, DateOnly> Validity
    {
        get => _validity;
        set
        {
            EnsureDateFromIsLesserThanDateTo(value);
            _validity = value;
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
    
    private static void EnsureLabelHasNoSymbols(string label)
    {
        if (!label.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
        {
            throw new ArgumentException("Label format is invalid, it can't contain symbols");
        }
    }
    
    private static void EnsureLabelLengthIsLesserOrEqualThan20(string label)
    {
        if (label.Length > 20)
        {
            throw new ArgumentException("Label format is invalid, length must be lesser or equal than 20");
        }
    }
    
    private static void EnsureDateFromIsLesserThanDateTo(Tuple<DateOnly, DateOnly> validity)
    {
        if (validity.Item1 >= validity.Item2)
        {
            throw new ArgumentException("DateFrom must be lesser than DateTo");
        }
    }

    public Promotion(string label, int discount, DateOnly from, DateOnly to)
    {
        Label = label;
        Discount = discount;
        Validity = new Tuple<DateOnly, DateOnly>(from, to);
    }
}