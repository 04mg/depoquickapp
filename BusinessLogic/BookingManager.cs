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
        foreach (var b in Bookings)
        {
            if (b.Deposit == booking.Deposit && b.Client == booking.Client)
            {
                if(booking.Duration.Item1 >= b.Duration.Item1 && booking.Duration.Item2 <= b.Duration.Item2)
                {
                    throw new ArgumentException("User already has a booking for this period.");
                }
                if(booking.Duration.Item1 >= b.Duration.Item1 && booking.Duration.Item2 <= b.Duration.Item2)
                {
                    throw new ArgumentException("User already has a booking for this period.");
                }
            }
        }
        booking.Id = NextBookingId;
        Bookings.Add(booking);
    }
    
    private int NextBookingId => Bookings.Count > 0 ? Bookings.Max(d => d.Id) + 1 : 1;

    public bool Exists(int id)
    {
        return Bookings.Any(b => b.Id == id);
    }

    public List<Booking> GetBookingsByEmail(string email, Credentials credentials)
    {
        if (credentials.Rank == "Administrator" || credentials.Email == email)
        {
            return Bookings.Where(b => b.Client.Email == email).ToList();
        }
        else
        {
            if (credentials.Rank != "Administrator")
            {
                throw new UnauthorizedAccessException("You are not authorized to perform this action.");
            }
            return new List<Booking>();
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