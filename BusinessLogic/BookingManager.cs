namespace BusinessLogic;

public class BookingManager
{
    public List<Booking> Bookings { get; set; }
    
    public BookingManager()
    {
        Bookings = new List<Booking>();
    }

    public void Add(AddBookingDto addBookingDto, DepositManager depositManager, AuthManager authManager)
    {
        var booking = new Booking(NextBookingId, addBookingDto.DepositId, addBookingDto.Email, addBookingDto.DateFrom, addBookingDto.DateTo, depositManager, authManager);
        Bookings.Add(booking);
    }
    
    private int NextBookingId => Bookings.Count > 0 ? Bookings.Max(d => d.Id) + 1 : 1;

    public bool Exists(int i)
    {
        return Bookings.Any(b => b.Id == i);
    }

    public List<Booking> GetBookingsByEmail(string email, Credentials credentials)
    {
        throw new NotImplementedException();
    }
}