using BusinessLogic.Domain;

namespace BusinessLogic.Repositories;

public interface IBookingRepository : IRepository<int, Booking>
{
    IEnumerable<Booking> GetAll();
    bool Exists(int id);
}