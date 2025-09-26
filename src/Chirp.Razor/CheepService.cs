using Chirp.Razor.wwwroot;
using Microsoft.Data.Sqlite;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    IDBFacade<CheepViewModel> _dbFacade;
    public CheepService()
    {
        var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        _dbFacade = new DBFacade(dbPath);
    }
    public List<CheepViewModel> GetCheeps()
    {
        return _dbFacade.Read(0, 32);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return _dbFacade.ReadByName(author);
        
        
        
    }

    

}
