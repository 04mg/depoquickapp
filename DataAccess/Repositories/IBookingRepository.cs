using Domain;

namespace DataAccess.Repositories;

public interface IBookingRepository : IRepository<int, Booking>
{
    void Update(Booking booking);
    IEnumerable<Booking> GetAll();
    bool Exists(int id);
}