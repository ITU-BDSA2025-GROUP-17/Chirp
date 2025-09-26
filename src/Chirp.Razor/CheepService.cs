using Chirp.Razor.wwwroot;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
}

public class CheepService : ICheepService
{
    IDBFacade<CheepViewModel> _dbFacade;
    public CheepService()
    {
        var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH")
                     ?? Path.Combine(Path.GetTempPath(), "chirp.db");

        _dbFacade = new CheepDBFacade(dbPath);
    }
    public List<CheepViewModel> GetCheeps(int page)
    {
        return _dbFacade.Read((page - 1) * 32, 32);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        return _dbFacade.ReadByName(author, (page - 1) * 32, 32);
        
        
        
    }

    

}
