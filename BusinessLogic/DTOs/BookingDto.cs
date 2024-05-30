namespace BusinessLogic.DTOs;

public class BookingDto
{
    public int Id { set; get; }
    public DateOnly DateFrom { set; get; }
    public DateOnly DateTo { set; get; }
    public string DepositName { set; get; } = string.Empty;
    public string Email { set; get; } = string.Empty;
    public string Stage { set; get; } = string.Empty;
    public string Message { set; get; } = string.Empty;
}