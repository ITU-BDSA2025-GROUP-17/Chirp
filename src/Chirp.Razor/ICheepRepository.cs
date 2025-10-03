namespace Chirp.Razor.wwwroot;

public interface ICheepRepository
{
    Task CreateCheep(CheepDTO cheep);
    Task<List<CheepDTO>> ReadCheeps(string? user, int offset, int count);
    Task UpdateCheep(CheepDTO cheep);
}   