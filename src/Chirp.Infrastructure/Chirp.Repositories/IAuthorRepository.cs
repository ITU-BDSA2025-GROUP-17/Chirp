using Microsoft.AspNetCore.Identity;

namespace Chirp.Repositories;

using Core;

/// <summary>
/// Defines the contract for accessing and manipulating author-related data.
/// Implementations handle author creation, follow relationships, and author queries.
/// </summary>
public interface IAuthorRepository
{
    /// <summary>
    /// Retrieves an author by their username.
    /// </summary>
    /// <param name="authorName">The unique username of the author.</param>
    /// <returns>An <see cref="AuthorDTO"/> if found; otherwise <c>null</c>.</returns>
    Task<AuthorDTO?> GetAuthorByName(string authorName);

    /// <summary>
    /// Retrieves an author by their email address.
    /// </summary>
    /// <param name="authorEmail">The email address of the author.</param>
    /// <returns>An <see cref="AuthorDTO"/> if found; otherwise <c>null</c>.</returns>
    Task<AuthorDTO?> GetAuthorByEmail(string authorEmail);

    /// <summary>
    /// Creates a new author.
    /// </summary>
    /// <param name="authorName">The username of the new author.</param>
    /// <param name="authorEmail">The email address of the new author.</param>
    Task CreateAuthor(string authorName, string authorEmail);

    /// <summary>
    /// Creates a follow relationship where one author follows another.
    /// </summary>
    /// <param name="userAuthor">The author who wants to follow another author.</param>
    /// <param name="followAuthor">The author to be followed.</param>
    Task Follow(AuthorDTO userAuthor, AuthorDTO followAuthor);

    /// <summary>
    /// Removes an existing follow relationship between two authors.
    /// </summary>
    /// <param name="userAuthor">The author who wants to unfollow.</param>
    /// <param name="unfollowAuthor">The author to be unfollowed.</param>
    Task UnFollow(AuthorDTO userAuthor, AuthorDTO unfollowAuthor);

    /// <summary>
    /// Determines whether one author is following another author.
    /// </summary>
    /// <param name="userAuthor">The author performing the check.</param>
    /// <param name="followAuthor">The author that may be followed.</param>
    /// <returns><c>true</c> if the author is following; otherwise <c>false</c>.</returns>
    Task<bool> IsFollowing(AuthorDTO userAuthor, AuthorDTO followAuthor);

    /// <summary>
    /// Retrieves all authors that the given author is following.
    /// </summary>
    /// <param name="userAuthor">The author whose followings should be retrieved.</param>
    /// <returns>A list of followed authors. The list is empty if none are followed.</returns>
    Task<List<AuthorDTO>> GetFollowing(AuthorDTO userAuthor);

    /// <summary>
    /// Deletes an author and removes all related follow relationships.
    /// </summary>
    /// <param name="userAuthor">The author to delete.</param>
    /// <returns>An <see cref="IdentityResult"/> indicating whether the deletion succeeded.</returns>
    Task<IdentityResult> DeleteAuthor(AuthorDTO userAuthor);

}