using Microsoft.EntityFrameworkCore;
namespace Chirp.Repositories;

using System.Runtime.CompilerServices;
using Core;

/// <summary>
/// Repository responsible for accessing and manipulating cheep-related data.
/// Handles creation, retrieval, updating, saving, and deletion of cheeps.
/// </summary>
public class CheepRepository : ICheepRepository
{
    // Database context used for all cheep-related operations
    private readonly CheepDBContext _dbContext;
    /// <summary>
    /// Initializes a new instance of the <see cref="CheepRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CheepRepository(CheepDBContext context)
    {
        _dbContext = context;
    }

    /// <summary>
    /// Creates and persists a new cheep written by an author.
    /// </summary>
    /// <param name="cheep">The cheep to create.</param>
    public async Task CreateCheep(CheepDTO cheep)
    {
        // Validate input cheep
        if (cheep.Author == null) throw new NullReferenceException("Author is null");
        // Ignore cheeps longer than 160 characters
        if (cheep.Text.Length > 160) return; 
        // Map DTO to Cheep entity
        Cheep newCheep = new()
        {
            Author = await _dbContext.Users.FindAsync(cheep.Author.AuthorId),
            Text = cheep.Text,
            TimeStamp = cheep.TimeStamp
        };
        // Add cheep to the DbContext (not yet persisted)
        await _dbContext.Cheeps.AddAsync(newCheep); 
        // Persist the cheep to the database
        await _dbContext.SaveChangesAsync(); 
    }

    /// <summary>
    /// Saves (bookmarks) a cheep for a specific author.
    /// </summary>
    /// <param name="user">The author saving the cheep.</param>
    /// <param name="cheep">The cheep to save.</param>
    public async Task SaveCheep(AuthorDTO user, CheepDTO cheep)
    {
        // Validate input data
        if (cheep.CheepId == null) throw new NullReferenceException("Cheep ID is null!");
        if (cheep.Author == null) throw new NullReferenceException("Cheep author is null!");
        // Create a new SavedCheep relationship
        SavedCheep newCheep = new()
        {
            Saver = await _dbContext.Users.FindAsync(user.AuthorId),
            CheepId = (long)cheep.CheepId,
            TimeStamp = DateTime.Now
        };
        await _dbContext.SavedCheeps.AddAsync(newCheep); 
        await _dbContext.SaveChangesAsync(); 
    }

    /// <summary>
    /// Removes a previously saved cheep for an author.
    /// </summary>
    /// <param name="user">The author removing the saved cheep.</param>
    /// <param name="cheep">The cheep to remove.</param>
    public async Task RemoveSavedCheep(AuthorDTO user, CheepDTO cheep)
    {
        // Find the saved cheep relationship
        var savedCheep = await _dbContext.SavedCheeps
            .FirstOrDefaultAsync(
                s => s.Saver!.Id == user.AuthorId
                && s.CheepId == cheep.CheepId
            );

        // If no saved cheep exists, nothing needs to be done
        if (savedCheep == null) return;

        _dbContext.SavedCheeps.Remove(savedCheep);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves cheeps, optionally filtered by author.
    /// </summary>
    /// <param name="user">Optional username to filter cheeps.</param>
    /// <param name="offset">Number of cheeps to skip.</param>
    /// <param name="count">Maximum number of cheeps to return.</param>
    /// <returns>A list of cheeps ordered by timestamp.</returns>
    public async Task<List<CheepDTO>> ReadCheeps(string? user, int offset, int count)
    {
        // Build query to retrieve cheeps from the database
        var query = from message in _dbContext.Cheeps
                    where message.Author!.UserName == user || user == null
                    orderby message.TimeStamp descending
                    select new CheepDTO
                    {
                        Text = message.Text,
                        TimeStamp = message.TimeStamp,
                        CheepId = message.CheepId,
                        Author = new()
                        {
                            AuthorId = message.Author!.Id,
                            Name = message.Author.UserName!,
                            Email = message.Author.Email!
                        }
                    };

        // Execute the query and return paginated results
        return await query.Skip(offset).Take(count).ToListAsync();
    }

    /// <summary>
    /// Retrieves cheeps saved by a specific author.
    /// </summary>
    /// <param name="authorId">The ID of the author.</param>
    /// <param name="offset">Number of cheeps to skip.</param>
    /// <param name="count">Maximum number of cheeps to return.</param>
    public async Task<List<CheepDTO>> ReadSavedCheeps(int authorId, int offset, int count)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = from save in _dbContext.SavedCheeps
                    join cheep in _dbContext.Cheeps on save.CheepId equals cheep.CheepId
                    where save.Saver!.Id == authorId
                    orderby save.TimeStamp descending
                    select new CheepDTO
                    {
                        Text = cheep.Text,
                        TimeStamp = cheep.TimeStamp,
                        CheepId = cheep.CheepId,
                        Author = new()
                        {
                            AuthorId = cheep.Author!.Id,
                            Name = cheep.Author.UserName!,
                            Email = cheep.Author.Email!
                        }
                    };

        // Execute the query and return the results
        return await query.Skip(offset).Take(count).ToListAsync();
    }

    /// <summary>
    /// Retrieves cheeps filtered by author and search text.
    /// </summary>
    /// <param name="user">Optional username to filter cheeps.</param>
    /// <param name="search">Text to search for in cheeps.</param>
    /// <param name="offset">Number of cheeps to skip.</param>
    /// <param name="count">Maximum number of cheeps to return.</param>
    public async Task<List<CheepDTO>> ReadCheepsWithSearch(string? user, string? search, int offset, int count)
    {
        // Ensure search string is not null
        if (search == null)
        {
            search = "";
        }
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = from message in _dbContext.Cheeps
                    where (message.Author!.UserName == user || user == null) && message.Text.Contains(search)
                    orderby message.TimeStamp descending
                    select new CheepDTO
                    {
                        Text = message.Text,
                        TimeStamp = message.TimeStamp,
                        CheepId = message.CheepId,
                        Author = new()
                        {
                            AuthorId = message.Author!.Id,
                            Name = message.Author.UserName!,
                            Email = message.Author.Email!
                        }
                    };

        // Execute the query and store the results
        var result = await query.Skip(offset).Take(count).ToListAsync();
        return result;
    }

    /// <summary>
    /// Updates the text of an existing cheep.
    /// </summary>
    /// <param name="cheep">The cheep containing updated data.</param>
    public async Task UpdateCheep(CheepDTO cheep)
    {
        // Retrieve the cheep entity to update
        var query = from message in _dbContext.Cheeps
                    where message.CheepId == cheep.CheepId
                    select message;

        var result = await query.ToListAsync();
        // Update cheep text if found
        if (result.Count < 1)
        {
            result[0].Text = cheep.Text;
            _dbContext.SaveChanges();
        }
    }

    /// <summary>
    /// Retrieves cheeps written by authors that the user follows.
    /// </summary>
    /// <param name="userNames">Usernames of followed authors.</param>
    /// <param name="offset">Number of cheeps to skip.</param>
    /// <param name="count">Maximum number of cheeps to return.</param>
    public async Task<List<CheepDTO>> ReadCheepsFromFollowers(List<string> userNames, int offset, int count)
    {

        var query = from message in _dbContext.Cheeps

                    where userNames.Contains(message.Author!.UserName!)
                    orderby message.TimeStamp descending
                    select new CheepDTO
                    {
                        Text = message.Text,
                        TimeStamp = message.TimeStamp,
                        CheepId = message.CheepId,
                        Author = new()
                        {
                            AuthorId = message.Author!.Id,
                            Name = message.Author.UserName!,
                            Email = message.Author.Email!
                        }
                    };


        // Execute the query and store the results
        var result = await query.Skip(offset).Take(count).ToListAsync();
        return result;
    }

    /// <summary>
    /// Retrieves a single cheep by its unique identifier.
    /// </summary>
    /// <param name="cheepId">The ID of the cheep.</param>
    /// <returns>The cheep if found; otherwise <c>null</c>.</returns>
    public async Task<CheepDTO?> GetCheepById(long cheepId)
    {
        var cheep = await _dbContext.Cheeps.Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.CheepId == cheepId);

        if (cheep == null) return null;

        return new CheepDTO
        {
            Text = cheep.Text,
            TimeStamp = cheep.TimeStamp,
            CheepId = cheep.CheepId,
            Author = new()
            {
                AuthorId = cheep.Author!.Id,
                Name = cheep.Author.UserName!,
                Email = cheep.Author.Email!
            }
        };
    }

    /// <summary>
    /// Deletes all saved cheeps for a given author.
    /// </summary>
    /// <param name="userName">The username of the author.</param>
    public async Task DeleteSavedCheeps(string userName)
    {
        var query = from author in _dbContext.Users
            .Include(a => a.SavedCheeps)
                    where author.UserName == userName
                    select author;

        var user = await query.FirstOrDefaultAsync();
        if (user == null) throw new NullReferenceException("resulting author is null");

        if (user.SavedCheeps != null && user.SavedCheeps.Any())
        {
            _dbContext.SavedCheeps.RemoveRange(user.SavedCheeps);
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes all cheeps written by a given author, including related saved cheeps.
    /// </summary>
    /// <param name="userName">The username of the author.</param>
    public async Task DeleteCheeps(string userName)
    {
        var query = from author in _dbContext.Users
            .Include(a => a.Cheeps)
                    where author.UserName == userName
                    select author;

        var user = await query.FirstOrDefaultAsync();
        if (user == null) throw new NullReferenceException("resulting author is null");

        if (user.Cheeps == null) return;

        // Collect IDs of all cheeps written by the user
        var cheepIds = user.Cheeps.Select(c => c.CheepId).ToHashSet();

        // Delete saved cheep entries referencing these cheeps first
        var savedCheeps = await _dbContext.SavedCheeps
            .Where(s => cheepIds.Contains(s.CheepId))
            .ToListAsync();

        _dbContext.SavedCheeps.RemoveRange(savedCheeps);

        _dbContext.Cheeps.RemoveRange(user.Cheeps!);

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Determines whether a cheep is saved by a specific author.
    /// </summary>
    /// <param name="user">The author.</param>
    /// <param name="cheep">The cheep to check.</param>
    /// <returns><c>true</c> if the cheep is saved; otherwise <c>false</c>.</returns>
    public async Task<bool> IsSaved(AuthorDTO user, CheepDTO cheep)
    {
        return await _dbContext.SavedCheeps.AnyAsync(
            save => save.Saver!.Id == user.AuthorId
            && save.CheepId == cheep.CheepId
        );
    }
}