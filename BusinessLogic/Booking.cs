namespace BusinessLogic;

public class Booking
{
    private readonly IPriceCalculator _priceCalculator;
    private Tuple<DateOnly, DateOnly> _duration = new(new DateOnly(), new DateOnly());
    public int Id { get; set; }
    public Deposit Deposit { get; }
    public User Client { get; }
    public string Message { get; set; } = "";
    public BookingStage Stage { get; set; } = BookingStage.Pending;

    public Tuple<DateOnly, DateOnly> Duration
    {
        get => _duration;
        set
        {
            EnsureDateFromIsLesserThanDateTo(value.Item1, value.Item2);
            EnsureDateFromIsGreaterThanToday(value.Item1);
            _duration = value;
        }
    }


    private static void EnsureDateFromIsLesserThanDateTo(DateOnly dateFrom, DateOnly dateTo)
    {
        if (dateFrom > dateTo)
        {
            throw new ArgumentException("The starting date of the booking must not be later than the ending date.");
        }
    }

    private static void EnsureDateFromIsGreaterThanToday(DateOnly dateFrom)
    {
        if (dateFrom < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ArgumentException("The starting date of the booking must not be earlier than today.");
        }
    }

    public double CalculatePrice()
    {
        return _priceCalculator.CalculatePrice(Deposit, Duration);
    }

    public Booking(int id, Deposit deposit, User client, DateOnly dateFrom, DateOnly dateTo, IPriceCalculator priceCalculator)
    {
        Id = id;
        Deposit = deposit;
        Client = client;
        Duration = new Tuple<DateOnly, DateOnly>(dateFrom, dateTo);
        _priceCalculator = priceCalculator;
    }

    public void Approve()
    {
        Stage = BookingStage.Approved;
    }

    public void Reject(string message)
    {
        Stage = BookingStage.Rejected;
        Message = message;
    }
}

public enum BookingStage
{
    Approved,
    Pending,
    Rejected
}