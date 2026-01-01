namespace Chirp.Services;

using Repositories;

/// <summary>
/// Defines the contract for cheep-related business logic.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for coordinating cheep
/// operations between the web layer and the repository layer, enforcing
/// validation rules, and handling pagination.
/// </remarks>
public interface ICheepService
{
    /// <summary>
    /// Retrieves publicly visible cheeps with optional search filtering.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="searchQuery">
    /// Optional search query used to filter cheeps.
    /// </param>
    /// <returns>
    /// A list of public cheeps for the specified page.
    /// </returns>
    Task<List<CheepDTO>> GetPublicCheeps(int pageNumber, string? searchQuery);

    /// <summary>
    /// Retrieves cheeps saved by a specific user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <returns>
    /// A list of saved cheeps.
    /// </returns>
    Task<List<CheepDTO>> GetUserTimelineCheeps(string user ,string author, int pageNumber);
    
    /// <summary>
    /// Retrieves cheeps saved by a specific user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <returns>
    /// A list of cheeps saved by the specified user.
    /// </returns>
    Task<List<CheepDTO>> GetSavedCheeps(string userName, int pageNumber);

    /// <summary>
    /// Creates a new cheep for a specific user.
    /// </summary>
    /// <param name="userName">Username of the author.</param>
    /// <param name="text">Text content of the cheep.</param>
    Task CreateCheepForUser(string userName, string text);

    /// <summary>
    /// Saves a cheep for a user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    /// <param name="cheepId">ID of the cheep to save.</param>
    Task SaveCheepForUser(string userName, long cheepId);

    /// <summary>
    /// Removes a saved cheep for a user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    /// <param name="cheepId">ID of the cheep to remove.</param>
    Task RemoveSavedCheepForUser(string userName, long cheepId);

    /// <summary>
    /// Determines whether a cheep is saved by a specific user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    /// <param name="cheepId">ID of the cheep.</param>
    /// <returns>
    /// <c>true</c> if the cheep is saved by the user; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> IsCheepSavedByUser(string userName, long cheepId);

    /// <summary>
    /// Deletes all saved cheeps for a specific user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    Task DeleteAllSavedCheepsForUser(string userName);

    /// <summary>
    /// Deletes all cheeps authored by a specific user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    Task DeleteAllCheepsForUser(string userName);
}
