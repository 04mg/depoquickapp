using BusinessLogic;
using BusinessLogic.DTOs;

namespace UI.Data;

public class BookingController
{
    private readonly DepoQuickApp _app;

    public BookingController(DepoQuickApp app)
    {
        _app = app;
    }

    public void AddBooking(AddBookingDto dto, Credentials credentials) => _app.AddBooking(dto, credentials);

    public double CalculateBookingPrice(AddBookingDto dto, Credentials credentials) =>
        _app.CalculateBookingPrice(dto, credentials);

    public List<BookingDto> ListAllBookings(Credentials credentials) => _app.ListAllBookings(credentials);
}