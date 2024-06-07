namespace DataAccess.Repositories;

public interface IRepository<in TKey, TValue>
{
    void Add(TValue value);
    TValue Get(TKey key);
}