using Domain;

namespace DataAccess.Repositories;

public interface IBookingRepository : IRepository<int, Booking>
{
    IEnumerable<Booking> GetAll();
    bool Exists(int id);
}