namespace Chirp.Razor.wwwroot;

public interface ICheepRepository
{
    Task CreateCheep(CheepDTO cheep);
    Task<List<CheepDTO>> ReadCheeps(string user);
    Task UpdateCheep(CheepDTO cheep);
}