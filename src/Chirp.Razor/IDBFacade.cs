using Microsoft.Data.Sqlite;

namespace Chirp.Razor.wwwroot;

public interface IDBFacade<T>
{
   
    public List<T> Read(int start, int limit);
    public List<T> ReadByName(string username);
    public void Store(T item);
}