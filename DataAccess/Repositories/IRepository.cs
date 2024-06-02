namespace DataAccess.Repositories;

public interface IRepository<in TKey, TValue>
{
    public void Add(TValue value);
    public TValue Get(TKey key);
}