namespace BusinessLogic;

public class Booking
{
    public int Id { get; }
    public int DepositId { get; }
    public string Email { get; }
    public Tuple<DateOnly, DateOnly> Duration { get; set; }

    public Booking(int id, int depositId, string email, DateOnly startDate, DateOnly endDate)
    {
        Id = id;
        DepositId = depositId;
        Email = email;
        Duration = new Tuple<DateOnly, DateOnly>(startDate, endDate);
    }
}