namespace Chirp.Razor.wwwroot;

public class CheepRepository : ICheepRepository
{
    public Task CreateCheep(CheepDTO cheep)
    {
        throw new NotImplementedException();
    }

    public Task<List<CheepDTO>> ReadCheeps(string user)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCheep(CheepDTO cheep)
    {
        throw new NotImplementedException();
    }
}