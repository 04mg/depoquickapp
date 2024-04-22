namespace BusinessLogic;

public class Promotion
{
    private string _label = "";
    private int _discount;
    private Tuple<DateOnly, DateOnly> _validity = new(new DateOnly(), new DateOnly());
    private const int MinDiscount = 5;
    private const int MaxDiscount = 70;
    private const int MaxLabelLength = 20;

    public string Label
    {
        get => _label;
        set
        {
            EnsureLabelHasNoSymbols(value);
            EnsureLabelLengthIsLesserOrEqualThan20(value);
            EnsureLabelIsNotEmpty(value);
            _label = value;
        }
    }

    public Tuple<DateOnly, DateOnly> Validity
    {
        get => _validity;
        set
        {
            EnsureDateFromIsLesserThanDateTo(value);
            EnsureDateToIsGreaterThanToday(value);
            _validity = value;
        }
    }

    public int Discount
    {
        get => _discount;
        set
        {
            if (value < MinDiscount || value > MaxDiscount)
            {
                throw new ArgumentException("Discount must be between 5% and 70%.");
            }

            _discount = value;
        }
    }

    private static void EnsureLabelHasNoSymbols(string label)
    {
        if (!label.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
        {
            throw new ArgumentException("Label must not contain symbols.");
        }
    }

    private static void EnsureLabelLengthIsLesserOrEqualThan20(string label)
    {
        if (label.Length > MaxLabelLength)
        {
            throw new ArgumentException("Label length must be lesser or equal than 20.");
        }
    }

    private static void EnsureDateFromIsLesserThanDateTo(Tuple<DateOnly, DateOnly> validity)
    {
        if (validity.Item1 >= validity.Item2)
        {
            throw new ArgumentException("The starting date of the promotion must not be later than the ending date.");
        }
    }
    
    private static void EnsureDateToIsGreaterThanToday(Tuple<DateOnly, DateOnly> validity)
    {
        if (validity.Item2 <= DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ArgumentException("The ending date of the promotion cannot be in the past.");
        }
    }
    
    private static void EnsureLabelIsNotEmpty(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException("Label must not be empty.");
        }
    }

    public Promotion(string label, int discount, DateOnly from, DateOnly to)
    {
        Label = label;
        Discount = discount;
        Validity = new Tuple<DateOnly, DateOnly>(from, to);
    }
}