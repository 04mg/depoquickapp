namespace BusinessLogic;

public class AddBookingDto
{
    public DateOnly DateFrom { get; init; }
    public DateOnly DateTo { get; init; }
    public int DepositId { get; init; }
    public string Email { get; init; }
}