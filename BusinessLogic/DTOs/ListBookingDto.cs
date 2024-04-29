namespace BusinessLogic;

public class ListBookingDto
{
    public int Id { get; init; }
    public DateOnly DateFrom { get; init; }
    public DateOnly DateTo { get; init; }
    public int DepositId { get; init; }
    public string Email { get; init; }
    public string Stage { get; init; }
    public string Message { get; init; }
}