namespace BusinessLogic.DTOs;

public class AddBookingDto
{
    public DateOnly DateFrom { init; get; }
    public DateOnly DateTo { init; get; }
    public int DepositId { init; get; }
    public string Email { init; get; } = string.Empty;
}