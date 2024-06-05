namespace BusinessLogic.DTOs;

public struct BookingDto
{
    public int Id { set; get; }
    public DateOnly DateFrom { set; get; }
    public DateOnly DateTo { set; get; }
    public string DepositName { set; get; }
    public string Email { set; get; }
    public string Stage { set; get; }
    public string Message { set; get; }
    public PaymentDto? Payment { set; get; }
}