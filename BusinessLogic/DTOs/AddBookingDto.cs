namespace BusinessLogic.DTOs;

public class AddBookingDto
{
    public DateOnly DateFrom { set; get; }
    public DateOnly DateTo { set; get; }
    public string DepositName { set; get; }
    public string Email { set; get; } = string.Empty;
}