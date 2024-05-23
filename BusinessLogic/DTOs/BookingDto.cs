namespace BusinessLogic.DTOs;

public class BookingDto
{
    public int Id { init; get; }
    public DateOnly DateFrom { init; get; }
    public DateOnly DateTo { init; get; }
    public string DepositName { init; get; } = string.Empty;
    public string Email { init; get; } = string.Empty;
    public string Stage { init; get; } = string.Empty;
    public string Message { init; get; } = string.Empty;
}