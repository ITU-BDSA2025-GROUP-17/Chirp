namespace Chirp.Services;

using Microsoft.AspNetCore.Identity;
using Repositories;

/// <summary>
/// Defines the contract for author-related business logic.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for validating authors,
/// enforcing application rules, and coordinating author operations between
/// the web layer and the repository layer.
/// </remarks>
public interface IAuthorService
{
    /// <summary>
    /// Retrieves an author by username.
    /// </summary>
    /// <param name="userName">The username of the author.</param>
    /// <returns>
    /// The corresponding <see cref="AuthorDTO"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<AuthorDTO?> GetAuthorByName(string userName);

    /// <summary>
    /// Retrieves an author by email address.
    /// </summary>
    /// <param name="email">The email address of the author.</param>
    /// <returns>
    /// The corresponding <see cref="AuthorDTO"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<AuthorDTO?> GetAuthorByEmail(string email);

    /// <summary>
    /// Creates a new author.
    /// </summary>
    /// <param name="userName">The username of the author.</param>
    /// <param name="email">The email address of the author.</param>
    Task CreateAuthor(string userName, string email);

    /// <summary>
    /// Creates a follow relationship between two users.
    /// </summary>
    /// <param name="currentUserName">Username of the user initiating the follow.</param>
    /// <param name="targetUserName">Username of the user to be followed.</param>
    Task FollowUser(string currentUserName, string targetUserName);

    /// <summary>
    /// Removes a follow relationship between two users.
    /// </summary>
    /// <param name="currentUserName">Username of the user initiating the unfollow.</param>
    /// <param name="targetUserName">Username of the user to be unfollowed.</param>
    Task UnfollowUser(string currentUserName, string targetUserName);

    /// <summary>
    /// Determines whether one user is following another.
    /// </summary>
    /// <param name="currentUserName">Username of the querying user.</param>
    /// <param name="targetUserName">Username of the target user.</param>
    /// <returns>
    /// <c>true</c> if the current user is following the target user;
    /// otherwise, <c>false</c>.
    /// </returns>
    Task<bool> IsFollowing(string currentUserName, string targetUserName);

    /// <summary>
    /// Retrieves all authors followed by a given user.
    /// </summary>
    /// <param name="userName">Username of the author.</param>
    /// <returns>
    /// A list of authors that the user is following.
    /// </returns>
    Task<List<AuthorDTO>> GetFollowing(string userName);

    /// <summary>
    /// Deletes an author account.
    /// </summary>
    /// <param name="userName">Username of the author to delete.</param>
    /// <returns>
    /// An <see cref="IdentityResult"/> indicating whether the deletion succeeded.
    /// </returns>
    Task<IdentityResult> DeleteAuthor(string userName);
}
