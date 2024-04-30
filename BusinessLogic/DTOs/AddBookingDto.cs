namespace BusinessLogic;

public class AddBookingDto
{
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public int DepositId { get; set; }
    public string Email { get; set; }
}