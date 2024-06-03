namespace Domain;

public class Promotion
{
    private const int MinDiscount = 5;
    private const int MaxDiscount = 70;
    private const int MaxLabelLength = 20;
    private int _discount;
    private string _label = "";
    private DateRange.DateRange _validity;

    public Promotion()
    {
    }

    public Promotion(int id, string label, int discount, DateOnly startDate, DateOnly endDate)
    {
        Id = id;
        Label = label;
        Discount = discount;
        Validity = new DateRange.DateRange(startDate, endDate);
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

    public DateRange.DateRange Validity
    {
        get => _validity;
        set
        {
            EnsureEndDateIsGreaterThanToday(value.EndDate);
            _validity = value;
        }
    }

    private void EnsureEndDateIsGreaterThanToday(DateOnly endDate)
    {
        if (endDate < DateOnly.FromDateTime(DateTime.Now))
            throw new ArgumentException("The ending date of the promotion cannot be in the past.");
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

    private static void EnsureLabelIsNotEmpty(string label)
    {
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException("Label must not be empty.");
    }
}