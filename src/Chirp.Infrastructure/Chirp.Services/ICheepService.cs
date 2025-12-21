namespace Chirp.Services;

using Repositories;
public interface ICheepService
{
    Task<List<CheepDTO>> GetPublicCheeps(int pageNumber, string? searchQuery);
    Task<List<CheepDTO>> GetUserTimelineCheeps(string userName, int pageNumber);
    Task<List<CheepDTO>> GetSavedCheeps(string userName, int pageNumber);
    Task CreateCheepForUser(string userName, string text);
    Task SaveCheepForUser(string userName, long cheepId);
    Task RemoveSavedCheepForUser(string userName, long cheepId);
    Task<bool> IsCheepSavedByUser(string userName, long cheepId);
    Task DeleteAllSavedCheepsForUser(string userName);
    Task DeleteAllCheepsForUser(string userName);
}