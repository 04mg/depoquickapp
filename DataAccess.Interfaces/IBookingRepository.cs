using Domain;

namespace DataAccess.Interfaces;

public interface IBookingRepository
{
    void Add(Booking booking);
    Booking Get(int id);
    void Update(Booking booking);
    IEnumerable<Booking> GetAll();
    bool Exists(int id);
}