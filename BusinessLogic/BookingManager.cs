namespace BusinessLogic;

public class BookingManager
{
    public List<Booking> Bookings { get; set; }
    
    public BookingManager()
    {
        Bookings = new List<Booking>();
    }

    public void Add(Booking booking)
    {
        EnsureNoOverlappingBooking(booking);
        booking.Id = NextBookingId;
        Bookings.Add(booking);
    }

    private void EnsureNoOverlappingBooking(Booking booking)
    {
        foreach (var oldBooking in Bookings)
        {
            var existBookingWithSameDepositAndUser = oldBooking.Deposit == booking.Deposit 
                                                     && oldBooking.Client == booking.Client;
            if (existBookingWithSameDepositAndUser)
            {
                EnsureNoOverlappingDates(booking, oldBooking);
            }
        }
    }

    private static void EnsureNoOverlappingDates(Booking booking, Booking oldBooking)
    {
        var isStartDateBetween = booking.Duration.Item1 >= oldBooking.Duration.Item1 && booking.Duration.Item1 <= oldBooking.Duration.Item2;
        var isEndDateBetween = booking.Duration.Item2 >= oldBooking.Duration.Item1 && booking.Duration.Item2 <= oldBooking.Duration.Item2;
        if(isEndDateBetween || isStartDateBetween)
        {
            throw new ArgumentException("User already has a booking for this period.");
        }
    }

    private int NextBookingId => Bookings.Count > 0 ? Bookings.Max(d => d.Id) + 1 : 1;

    public bool Exists(int id)
    {
        return Bookings.Any(b => b.Id == id);
    }

    public List<Booking> GetBookingsByEmail(string email, Credentials credentials)
    {
        EnsureUserIsAdministrator(credentials);
        EnsureEmailMatch(email, credentials); 
        return Bookings.Where(b => b.Client.Email == email).ToList();
    }

    private static void EnsureEmailMatch(string email, Credentials credentials)
    {
        if (credentials.Email == email)
        {
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
        }
    }

    private static void EnsureUserIsAdministrator(Credentials credentials)
    {
        if (credentials.Rank != "Administrator")
        {
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
        }
    }

    public List<Booking> GetAllBookings(Credentials credentials)
    {
        if (credentials.Rank == "Administrator")
        {
            return Bookings;
        }
        else
        {
            return new List<Booking>();
        }
    }

    public void Manage(int i, Credentials credentials, bool isApproved, string message = "")
    {
        if (credentials.Rank == "Administrator")
        {
            if (isApproved)
            {
                Bookings.First(b => b.Id == i).Stage = BookingStage.Approved;
            }

            if (!isApproved)
            {
                Bookings.First(b => b.Id == i).Stage = BookingStage.Rejected;
                if (!String.IsNullOrEmpty(message))
                {
                    Bookings.First(b => b.Id == i).Message = message;
                }
            }
            
        }
    }
}