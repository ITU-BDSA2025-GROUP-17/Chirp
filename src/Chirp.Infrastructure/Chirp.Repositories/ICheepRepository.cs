namespace Chirp.Repositories;

using Core;

/// <summary>
/// Defines the contract for accessing and manipulating cheep-related data.
/// Implementations handle creation, retrieval, updating, saving, and deletion of cheeps.
/// </summary>
public interface ICheepRepository
{
    /// <summary>
    /// Creates and persists a new cheep written by an author.
    /// </summary>
    /// <param name="cheep">The cheep to create.</param>
    Task CreateCheep(CheepDTO cheep);

    /// <summary>
    /// Saves (bookmarks) a cheep for a specific author.
    /// </summary>
    /// <param name="user">The author saving the cheep.</param>
    /// <param name="cheep">The cheep to save.</param>
    Task SaveCheep(AuthorDTO user, CheepDTO cheep);

    /// <summary>
    /// Removes a previously saved cheep for an author.
    /// </summary>
    /// <param name="user">The author removing the saved cheep.</param>
    /// <param name="cheep">The cheep to remove.</param>
    Task RemoveSavedCheep(AuthorDTO user, CheepDTO cheep);

    /// <summary>
    /// Retrieves cheeps, optionally filtered by author.
    /// </summary>
    /// <param name="user">Optional username to filter cheeps.</param>
    /// <param name="offset">Number of cheeps to skip.</param>
    /// <param name="count">Maximum number of cheeps to return.</param>
    /// <returns>A list of cheeps ordered by timestamp.</returns>
    Task<List<CheepDTO>> ReadCheeps(string? user, int offset, int count);

    /// <summary>
    /// Retrieves cheeps saved by a specific author.
    /// </summary>
    /// <param name="authorId">The ID of the author.</param>
    /// <param name="offset">Number of cheeps to skip.</param>
    /// <param name="count">Maximum number of cheeps to return.</param>
    Task<List<CheepDTO>> ReadSavedCheeps(int authorId, int offset, int count);

    /// <summary>
    /// Retrieves cheeps filtered by author and search text.
    /// </summary>
    /// <param name="user">Optional username to filter cheeps.</param>
    /// <param name="search">Text to search for in cheeps.</param>
    /// <param name="offset">Number of cheeps to skip.</param>
    /// <param name="count">Maximum number of cheeps to return.</param>
    Task<List<CheepDTO>> ReadCheepsWithSearch(string? user, string? search, int offset, int count);

    /// <summary>
    /// Updates the text of an existing cheep.
    /// </summary>
    /// <param name="cheep">The cheep containing updated data.</param>
    Task UpdateCheep(CheepDTO cheep);

    /// <summary>
    /// Retrieves cheeps written by authors that the user follows.
    /// </summary>
    /// <param name="userNames">Usernames of followed authors.</param>
    /// <param name="offset">Number of cheeps to skip.</param>
    /// <param name="count">Maximum number of cheeps to return.</param>
    Task<List<CheepDTO>> ReadCheepsFromFollowers(List<string> userNames, int offset, int count);

    /// <summary>
    /// Determines whether a cheep is saved by a specific author.
    /// </summary>
    /// <param name="user">The author.</param>
    /// <param name="cheep">The cheep to check.</param>
    /// <returns><c>true</c> if the cheep is saved; otherwise <c>false</c>.</returns>
    Task<bool> IsSaved(AuthorDTO user, CheepDTO cheep);

    /// <summary>
    /// Retrieves a single cheep by its unique identifier.
    /// </summary>
    /// <param name="cheepId">The ID of the cheep.</param>
    /// <returns>The cheep if found; otherwise <c>null</c>.</returns>
    Task<CheepDTO?> GetCheepById(long cheepId);

    /// <summary>
    /// Deletes all saved cheeps for a given author.
    /// </summary>
    /// <param name="user">The username of the author.</param>
    Task DeleteSavedCheeps(string user);

    /// <summary>
    /// Deletes all cheeps written by a given author, including related saved cheeps.
    /// </summary>
    /// <param name="user">The username of the author.</param>
    Task DeleteCheeps(string user);

}