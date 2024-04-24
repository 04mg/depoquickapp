namespace BusinessLogic;

public class Booking
{
    private Tuple<DateOnly, DateOnly> _duration = new(new DateOnly(), new DateOnly());
    public int Id { get; }
    public int DepositId { get; }
    public string Email { get; }
    
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

    private static void EnsureDepositExists(int depositId, DepositManager depositManager)
    {
        if (depositManager.Deposits.All(d => d.Id != depositId))
        {
            throw new ArgumentException("The deposit does not exist.");
        }
    }

    private static void EnsureUserExists(string email, AuthManager authManager)
    {
        if (!authManager.Exists(email))
        {
            throw new ArgumentException("The user does not exist.");
        }
    }

    public Booking(int id, int depositId, string email, DateOnly startDate, DateOnly endDate,
        DepositManager depositManager, AuthManager authManager)
    {
        Id = id;
        EnsureDepositExists(depositId, depositManager);
        DepositId = depositId;
        EnsureUserExists(email, authManager);
        Email = email;
        Duration = new Tuple<DateOnly, DateOnly>(startDate, endDate);
    }
}

public enum BookingStage
{
    Approved,
    Pending,
    Rejected
}