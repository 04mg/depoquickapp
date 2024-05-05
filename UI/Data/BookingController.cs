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
}