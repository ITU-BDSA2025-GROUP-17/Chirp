using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Chirp.Repositories;

using System.Net.Security;
using Core;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.VisualBasic;
    /// <summary>
    /// Repository responsible for accessing and manipulating author-related data.
    /// Provides operations for creating authors, following/unfollowing, and querying author information.
    /// </summary>
public class AuthorRepository : IAuthorRepository
{
    private readonly CheepDBContext _dbContext;
    public AuthorRepository(CheepDBContext context)
    {
        _dbContext = context;

    }
    /// <summary>
    /// Retrieves an author by their username.
    /// </summary>
    /// <param name="authorName">The unique username of the author.</param>
    /// <returns>An <see cref="AuthorDTO"/> if found; otherwise <c>null</c>.</returns>
    public async Task<AuthorDTO?> GetAuthorByName(string authorName)
    {
        // Query the database for an author with the given username and include their cheeps
        var query = from author in _dbContext.Users
                .Include(a => a.Cheeps)
                    where author.UserName == authorName
                    select author;

        // Return null if no author was found
        var result = await query.FirstOrDefaultAsync();
        if (result == null) return null;

        // Map the Author entity to an AuthorDTO
        return new AuthorDTO
        {
            Name = result.UserName!,
            AuthorId = result.Id,
            Email = result.Email!,
            Messages = result.Cheeps!.Select(m => new CheepDTO
            {
                CheepId = m.CheepId,
                Text = m.Text,
                TimeStamp = m.TimeStamp,
                Author = new AuthorDTO
                {
                    Name = result.UserName!,
                    AuthorId = result.Id,
                    Email = result.Email!
                }
            }).ToList()
        };
    }
    /// <summary>
    /// Retrieves an author by their email address.
    /// </summary>
    /// <param name="authorEmail">The email address of the author.</param>
    /// <returns>An <see cref="AuthorDTO"/> if found; otherwise <c>null</c>.</returns>
    public async Task<AuthorDTO?> GetAuthorByEmail(string authorEmail)
    {
        // Query the database for an author with the given email address
        var query = from author in _dbContext.Users
                    where author.Email == authorEmail
                    select author
        ;

        // Return null if no author was found
        var result = await query.FirstOrDefaultAsync();
        if (result == null) return null;

        // Map the Author entity to an AuthorDTO
        return new AuthorDTO
        {
            Name = result.UserName!,
            AuthorId = result.Id,
            Email = result.Email!,
            Messages = result.Cheeps!.Select(m => new CheepDTO
            {
                CheepId = m.CheepId,
                Text = m.Text,
                TimeStamp = m.TimeStamp,
                Author = new AuthorDTO
                {
                    Name = result.UserName!,
                    AuthorId = result.Id,
                    Email = result.Email!
                }
            }).ToList()
        };
    }
    /// <summary>
    /// Creates a new author and persists it in the database.
    /// </summary>
    /// <param name="authorName">The username of the new author.</param>
    /// <param name="authorEmail">The email address of the new author.</param>
    public async Task CreateAuthor(string authorName, string authorEmail)
    {
        // Create a new Author entity
        var newAuthor = new Author()
        {
            UserName = authorName,
            Email = authorEmail,
        };
        // Add the author to the DbContext (not yet persisted)
        await _dbContext.Users.AddAsync(newAuthor); // does not write to the database!
        // Persist the new author to the database
        await _dbContext.SaveChangesAsync(); // persist the changes in the database
    }
    /// <summary>
    /// Creates a follow relationship where one author follows another.
    /// </summary>
    /// <param name="userAuthor">The author who wants to follow another author.</param>
    /// <param name="followAuthor">The author to be followed.</param>
    public async Task Follow(AuthorDTO userAuthor, AuthorDTO followAuthor)
    {
        if (userAuthor == null) throw new NullReferenceException("userAuthor is null");
        if (followAuthor == null) throw new NullReferenceException("followAuthor is null");

        if (userAuthor.AuthorId == followAuthor.AuthorId) throw new InvalidOperationException("userAuthor cannot follow themself");

        // Load the user who wants to follow another author, including existing follow relations
        var userQuery = from author in _dbContext.Users
                .Include(a => a.Following)
                        where author.Id == userAuthor.AuthorId
                        select author;

        var user = await userQuery.FirstOrDefaultAsync();
        if (user == null) throw new NullReferenceException("resulting author is null");

        // Initialize the following list if it has not been loaded yet
        if (user.Following == null)
        {
            user.Following = new List<Follow>();
        }

        // Create and add a new follow relationship
        user.Following.Add(
            new Follow()
            {
                Follower = user,
                FollowingId = followAuthor.AuthorId
            }
        );

        await _dbContext.SaveChangesAsync();
    }
    /// <summary>
    /// Removes an existing follow relationship between two authors.
    /// </summary>
    /// <param name="userAuthor">The author who wants to unfollow.</param>
    /// <param name="unfollowAuthor">The author to be unfollowed.</param>
    public async Task UnFollow(AuthorDTO userAuthor, AuthorDTO unfollowAuthor)
    {
        if (userAuthor == null) throw new NullReferenceException("userAuthor is null");
        if (unfollowAuthor == null) throw new NullReferenceException("UnFollowAuthor is null");

        // Load the user who wants to unfollow another author
        var userQuery = from author in _dbContext.Users
                .Include(a => a.Following)
                        where author.Id == userAuthor.AuthorId
                        select author;

        var user = await userQuery.FirstOrDefaultAsync();
        if (user == null) throw new NullReferenceException("resulting author is null");


        // Load the author that should be unfollowed
        var unfollowQuery = from author in _dbContext.Users
                .Include(a => a.Following)
                            where author.Id == unfollowAuthor.AuthorId
                            select author;

        var unfollowing = await unfollowQuery.FirstOrDefaultAsync();
        if (unfollowing == null) throw new NullReferenceException("resulting author is null");

        // Find the follow relationship to remove
        var followToRemove = user.Following!.FirstOrDefault(f => f.FollowingId == unfollowing.Id);
        if (followToRemove != null)
        {
            // Remove the follow relationship from the database
            _dbContext.Remove(followToRemove);
            await _dbContext.SaveChangesAsync();
        }

    }
    /// <summary>
    /// Determines whether one author is following another author.
    /// </summary>
    /// <param name="userAuthor">The author performing the check.</param>
    /// <param name="followAuthor">The author that may be followed.</param>
    /// <returns><c>true</c> if the author is following; otherwise <c>false</c>.</returns>
    public async Task<bool> IsFollowing(AuthorDTO userAuthor, AuthorDTO followAuthor)
    {
        // Retrieve all authors the user is currently following
        var userFollowings = await GetFollowing(userAuthor);
        // Check if any followed author matches the given author
        foreach (var user in userFollowings)
        {
            if (user.AuthorId == followAuthor.AuthorId) return true;
        }

        return false;

    }
    /// <summary>
    /// Retrieves all authors that the given author is following.
    /// </summary>
    /// <param name="userAuthor">The author whose followings should be retrieved.</param>
    /// <returns>A list of authors the user is following. The list is empty if none are followed.</returns>
    public async Task<List<AuthorDTO>> GetFollowing(AuthorDTO userAuthor)
    {
        // Load the author and include all follow relationships
        var query = from author in _dbContext.Users
            .Include(a => a.Following)!
                .ThenInclude(f => f.Following)
                    where author.Id == userAuthor.AuthorId
                    select author;

        var result = await query.FirstOrDefaultAsync();

        if (result == null) throw new NullReferenceException("resulting author is null");
        // Return an empty list if the author follows no one
        if (result.Following == null || result.Following.Count == 0)
        {
            return new List<AuthorDTO>();
        }

        // Map followed authors to AuthorDTOs
        var followings = new List<AuthorDTO>();
        foreach (var follow in result.Following!)
        {
            followings.Add(
                new AuthorDTO
                {
                    Name = follow.Following!.UserName!,
                    Email = follow.Following!.Email!,
                    AuthorId = follow.Following!.Id
                }
            );
        }

        return followings;

    }

    /// <summary>
    /// Deletes an author and removes all related follow relationships.
    /// </summary>
    /// <param name="userAuthor">The author to delete.</param>
    /// <returns>An <see cref="IdentityResult"/> indicating whether the deletion succeeded.</returns>
    public async Task<IdentityResult> DeleteAuthor(AuthorDTO userAuthor)
    {
        if (userAuthor == null) throw new NullReferenceException("userAuthor is null");

        // Load the author to delete, including follow relationships
        var userQuery = from author in _dbContext.Users
            .Include(a => a.Following)
                        where author.Id == userAuthor.AuthorId
                        select author;

        var user = await userQuery.FirstOrDefaultAsync();

        if (user == null) throw new NullReferenceException("userAuthor is null");

        // Remove follow relationships where the user is the follower
        if (user.Following != null && user.Following.Any())
        {
            _dbContext.Follows.RemoveRange(user.Following);
        }

        // Load follow relationships where the user is being followed
        var followersOfUser = await _dbContext.Follows
            .Where(f => f.FollowingId == user.Id)
            .ToListAsync();

        // Remove follow relationships where the user is the followed author
        if (followersOfUser.Any())
        {
            _dbContext.Follows.RemoveRange(followersOfUser);
        }

        // Remove the author entity
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return IdentityResult.Success;
    }

}