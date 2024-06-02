using Domain;

namespace DataAccess;

public interface IBookingRepository : IRepository<int, Booking>
{
    IEnumerable<Booking> GetAll();
    bool Exists(int id);
}