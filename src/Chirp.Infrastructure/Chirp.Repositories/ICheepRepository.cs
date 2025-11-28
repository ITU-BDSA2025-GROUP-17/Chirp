namespace Chirp.Repositories;

using Core;

public interface ICheepRepository
{
    Task CreateCheep(CheepDTO cheep);
    Task<List<CheepDTO>> ReadCheeps(string? user, int offset, int count);
    Task<List<CheepDTO>> ReadCheepsWithSearch(string? user, string? search, int offset, int count);
    Task UpdateCheep(CheepDTO cheep);
    Task<List<CheepDTO>> ReadCheepsFromFollowers(string user, int offset, int count);
    Task DeleteCheeps(string user);

}