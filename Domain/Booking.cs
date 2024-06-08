using Domain.Enums;
using Domain.Exceptions;

namespace Domain;

public class Booking
{
    private readonly DateRange.DateRange _duration = new(new DateOnly(), new DateOnly());

    public Booking()
    {
    }

    public Booking(int id, Deposit deposit, User client, DateOnly startDate, DateOnly dateTo, Payment payment)
    {
        Id = id;
        Deposit = deposit;
        Client = client;
        Duration = new DateRange.DateRange(startDate, dateTo);
        Payment = payment;
        EnsureDurationIsContainedInDepositAvailabilityPeriods(Deposit, Duration);
        MakeDurationUnavailable(_duration);
    }

    public int Id { get; init; }
    public int DepositId { get; set; }
    public Deposit Deposit { get; set; }
    public int ClientId { get; set; }
    public User Client { get; set; }
    public string Message { get; set; } = "";
    public BookingStage Stage { get; set; } = BookingStage.Pending;
    public Payment? Payment { get; set; }

    public DateRange.DateRange Duration
    {
        get => _duration;
        private init
        {
            EnsureDateFromIsNotEqualToDateTo(value.StartDate, value.EndDate);
            EnsureDateFromIsGreaterThanToday(value.StartDate);
            _duration = value;
        }
    }

    private void MakeDurationUnavailable(DateRange.DateRange duration)
    {
        var dateRange = new DateRange.DateRange(duration.StartDate, duration.EndDate);
        Deposit.MakeUnavailable(dateRange);
    }

    private static void EnsureDurationIsContainedInDepositAvailabilityPeriods(Deposit deposit,
        DateRange.DateRange dateRange)
    {
        if (!deposit.IsAvailable(dateRange))
            throw new DomainException(
                "The duration of the booking must be contained in the deposit availability periods.");
    }

    private static void EnsureDateFromIsNotEqualToDateTo(DateOnly startDate, DateOnly dateTo)
    {
        if (startDate == dateTo)
            throw new DomainException("The starting date of the booking must not be the same as the ending date.");
    }

    private static void EnsureDateFromIsGreaterThanToday(DateOnly startDate)
    {
        if (startDate < DateOnly.FromDateTime(DateTime.Now))
            throw new DomainException("The starting date of the booking must not be earlier than today.");
    }

    public void Approve()
    {
        Stage = BookingStage.Approved;
        Payment?.Capture();
    }

    public void Reject(string message)
    {
        MakeDurationAvailable(Duration);
        Stage = BookingStage.Rejected;
        Message = message;
        Payment = null;
    }

    private void MakeDurationAvailable(DateRange.DateRange duration)
    {
        var dateRange = new DateRange.DateRange(duration.StartDate, duration.EndDate);
        Deposit.MakeAvailable(dateRange);
    }

    public bool IsPaymentCaptured()
    {
        return Payment is { Status: PaymentStatus.Captured };
    }

    public string GetPaymentStatus()
    {
        return Payment == null ? "Rejected" : Payment.Status.ToString();
    }

    public string GetDepositName()
    {
        return Deposit.Name;
    }

    public string GetClientEmail()
    {
        return Client.Email;
    }

    public int GetPromotionsCount()
    {
        return Deposit.GetPromotionsCount();
    }
}