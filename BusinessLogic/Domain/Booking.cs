using BusinessLogic.Calculators;
using BusinessLogic.Enums;

namespace BusinessLogic.Domain;

public class Booking
{
    private readonly Tuple<DateOnly, DateOnly> _duration = new(new DateOnly(), new DateOnly());
    private readonly IPriceCalculator _priceCalculator;

    public Booking(int id, Deposit deposit, User client, DateOnly dateFrom, DateOnly dateTo,
        IPriceCalculator priceCalculator)
    {
        Id = id;
        Deposit = deposit;
        Client = client;
        Duration = new Tuple<DateOnly, DateOnly>(dateFrom, dateTo);
        _priceCalculator = priceCalculator;
        EnsureDurationIsContainedInDepositAvailabilityPeriods(Deposit, Duration);
        MakeDurationUnavailable(_duration);
    }

    private void MakeDurationUnavailable(Tuple<DateOnly, DateOnly> duration)
    {
        var dateRange = new DateRange(duration.Item1, duration.Item2);
        Deposit.MakeUnavailable(dateRange);
    }

    public int Id { get; set; }
    public Deposit Deposit { get; }
    public User Client { get; }
    public string Message { get; private set; } = "";
    public BookingStage Stage { get; private set; } = BookingStage.Pending;

    public Tuple<DateOnly, DateOnly> Duration
    {
        get => _duration;
        private init
        {
            EnsureDateFromIsLesserThanDateTo(value.Item1, value.Item2);
            EnsureDateFromIsNotEqualToDateTo(value.Item1, value.Item2);
            EnsureDateFromIsGreaterThanToday(value.Item1);
            _duration = value;
        }
    }
    
    private void EnsureDurationIsContainedInDepositAvailabilityPeriods(Deposit deposit, Tuple<DateOnly, DateOnly> duration)
    {
        var dateRange = new DateRange(duration.Item1, duration.Item2);
        if (!deposit.IsAvailable(dateRange))
        {
            throw new ArgumentException("The duration of the booking must be contained in the deposit availability periods.");
        }
    }

    private static void EnsureDateFromIsNotEqualToDateTo(DateOnly dateFrom, DateOnly dateTo)
    {
        if (dateFrom == dateTo)
            throw new ArgumentException("The starting date of the booking must not be the same as the ending date.");
    }

    private static void EnsureDateFromIsLesserThanDateTo(DateOnly dateFrom, DateOnly dateTo)
    {
        if (dateFrom > dateTo)
            throw new ArgumentException("The starting date of the booking must not be later than the ending date.");
    }

    private static void EnsureDateFromIsGreaterThanToday(DateOnly dateFrom)
    {
        if (dateFrom < DateOnly.FromDateTime(DateTime.Now))
            throw new ArgumentException("The starting date of the booking must not be earlier than today.");
    }

    public double CalculatePrice()
    {
        return _priceCalculator.CalculatePrice(Deposit, Duration);
    }

    public void Approve()
    {
        Stage = BookingStage.Approved;
    }

    public void Reject(string message)
    {
        MakeDurationAvailable(Duration);
        Stage = BookingStage.Rejected;
        Message = message;
    }

    private void MakeDurationAvailable(Tuple<DateOnly, DateOnly> duration)
    {
        var dateRange = new DateRange(duration.Item1, duration.Item2);
        Deposit.MakeAvailable(dateRange);
    }
}