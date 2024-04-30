namespace BusinessLogic;

public class ListBookingDto
{
    public int Id { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public int DepositId { get; set; }
    public string Email { get; set; }
    public string Stage { get; set; }
    public string Message { get; set; }
}