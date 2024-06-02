namespace Domain;

public class Promotion
{
    private const int MinDiscount = 5;
    private const int MaxDiscount = 70;
    private const int MaxLabelLength = 20;
    private int _discount;
    private string _label = "";
    private Tuple<DateOnly, DateOnly> _validity = new(new DateOnly(), new DateOnly());

    public Promotion(int id, string label, int discount, DateOnly from, DateOnly to)
    {
        Id = id;
        Label = label;
        Discount = discount;
        Validity = new Tuple<DateOnly, DateOnly>(from, to);
    }

    public int Id { get; set; }

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
            EnsureDiscountIsWithinRange(value);
            _discount = value;
        }
    }

    private static void EnsureDiscountIsWithinRange(int value)
    {
        if (value < MinDiscount || value > MaxDiscount)
            throw new ArgumentException("Discount must be between 5% and 70%.");
    }

    private static void EnsureLabelHasNoSymbols(string label)
    {
        if (!label.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
            throw new ArgumentException("Label must not contain symbols.");
    }

    private static void EnsureLabelLengthIsLesserOrEqualThan20(string label)
    {
        if (label.Length > MaxLabelLength) throw new ArgumentException("Label length must be lesser or equal than 20.");
    }

    private static void EnsureDateFromIsLesserThanDateTo(Tuple<DateOnly, DateOnly> validity)
    {
        if (validity.Item1 >= validity.Item2)
            throw new ArgumentException("The starting date of the promotion must not be later than the ending date.");
    }

    private static void EnsureDateToIsGreaterThanToday(Tuple<DateOnly, DateOnly> validity)
    {
        if (validity.Item2 <= DateOnly.FromDateTime(DateTime.Now))
            throw new ArgumentException("The ending date of the promotion cannot be in the past.");
    }

    private static void EnsureLabelIsNotEmpty(string label)
    {
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException("Label must not be empty.");
    }
}