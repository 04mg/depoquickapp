namespace BusinessLogic;

public class Booking
{
    private Tuple<DateOnly, DateOnly> _duration = new(new DateOnly(), new DateOnly());
    public int Id { get; }
    public int DepositId { get; }
    public string Email { get; }

    public Tuple<DateOnly, DateOnly> Duration
    {
        get => _duration;
        set
        {
            EnsureDateFromIsLesserThanDateTo(value.Item1, value.Item2);
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

    public Booking(int id, int depositId, string email, DateOnly startDate, DateOnly endDate)
    {
        Id = id;
        DepositId = depositId;
        Email = email;
        Duration = new Tuple<DateOnly, DateOnly>(startDate, endDate);
    }
}