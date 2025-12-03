namespace Chirp.Repositories;

using Core;

public interface ICheepRepository
{
    Task CreateCheep(CheepDTO cheep);
    Task SaveCheep(AuthorDTO user, CheepDTO cheep);
    Task RemoveSavedCheep(AuthorDTO user, CheepDTO cheep);
    Task<List<CheepDTO>> ReadCheeps(string? user, int offset, int count);
    Task<List<CheepDTO>> ReadSavedCheeps(string? user, int offset, int count);
    
    Task<List<CheepDTO>> ReadCheepsWithSearch(string? user, string? search, int offset, int count);
    Task UpdateCheep(CheepDTO cheep);
    Task<List<CheepDTO>> ReadCheepsFromFollowers(string user, int offset, int count);

    Task<bool> IsSaved(AuthorDTO user, CheepDTO cheep);
    Task<CheepDTO?> GetCheepById(long cheepId);
    Task DeleteSavedCheeps(string user);
    Task DeleteCheeps(string user);

}