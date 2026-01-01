namespace Chirp.Services;

using Microsoft.AspNetCore.Identity;
using Repositories;

/// <summary>
/// Service responsible for author-related business logic.
/// </summary>
/// <remarks>
/// This service acts as an intermediary between the web layer and the
/// <see cref="IAuthorRepository"/>, enforcing business rules such as
/// validation, existence checks, and preventing invalid follow operations.
/// </remarks>
public class AuthorService : IAuthorService
{
    /// <summary>
    /// Repository used for author persistence and follow relationships.
    /// </summary>
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorService"/> class.
    /// </summary>
    /// <param name="authorRepository">
    /// Repository responsible for author data access.
    /// </param>
    public AuthorService(IAuthorRepository authorRepository, ICheepRepository cheepRepository)
    {
        _authorRepository = authorRepository;
        _cheepRepository = cheepRepository;
    }

    /// <summary>
    /// Retrieves an author by username.
    /// </summary>
    /// <param name="userName">The username of the author.</param>
    /// <returns>
    /// The corresponding <see cref="AuthorDTO"/> if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<AuthorDTO?> GetAuthorByName(string userName)
    {
        AuthorDTO? author = await _authorRepository.GetAuthorByName(userName);
        return author;
    }

    /// <summary>
    /// Retrieves an author by email address.
    /// </summary>
    /// <param name="email">The email address of the author.</param>
    /// <returns>
    /// The corresponding <see cref="AuthorDTO"/> if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<AuthorDTO?> GetAuthorByEmail(string email)
    {
        AuthorDTO? author = await _authorRepository.GetAuthorByEmail(email);
        return author;
    }

    /// <summary>
    /// Creates a new author.
    /// </summary>
    /// <param name="userName">The username of the author.</param>
    /// <param name="email">The email address of the author.</param>
    public async Task CreateAuthor(string userName, string email)
    {
        await _authorRepository.CreateAuthor(userName, email);
    }

    /// <summary>
    /// Creates a follow relationship between two users.
    /// </summary>
    /// <param name="currentUserName">Username of the user initiating the follow.</param>
    /// <param name="targetUserName">Username of the user to be followed.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if either user does not exist or if a user attempts to follow themselves.
    /// </exception>
    public async Task FollowUser(string currentUserName, string targetUserName)
    {
        AuthorDTO? currentUser = await _authorRepository.GetAuthorByName(currentUserName);
        if (currentUser == null)
        {
            throw new InvalidOperationException($"user with username: '{currentUserName}' doesn't exist");
        }

        AuthorDTO? targetUser = await _authorRepository.GetAuthorByName(targetUserName);
        if (targetUser == null)
        {
            throw new InvalidOperationException($"user with username: '{targetUserName}' doesn't exist");
        }

        if (currentUser.AuthorId == targetUser.AuthorId)
        {
            throw new InvalidOperationException($"You cannot follow yourself");
        }

        await _authorRepository.Follow(currentUser, targetUser);
    }

    /// <summary>
    /// Removes a follow relationship between two users.
    /// </summary>
    /// <param name="currentUserName">Username of the user initiating the unfollow.</param>
    /// <param name="targetUserName">Username of the user to be unfollowed.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if either user does not exist or if a user attempts to unfollow themselves.
    /// </exception>
    public async Task UnfollowUser(string currentUserName, string targetUserName)
    {
        AuthorDTO? currentUser = await _authorRepository.GetAuthorByName(currentUserName);
        if (currentUser == null)
        {
            throw new InvalidOperationException($"user with username: '{currentUserName}' doesn't exist");
        }

        AuthorDTO? targetUser = await _authorRepository.GetAuthorByName(targetUserName);
        if (targetUser == null)
        {
            throw new InvalidOperationException($"user with username: '{targetUserName}' doesn't exist");
        }

        // Prevent users from unfollowing themselves
        if (currentUser.AuthorId == targetUser.AuthorId)
        {
            throw new InvalidOperationException($"You cannot follow yourself");
        }

        await _authorRepository.UnFollow(currentUser, targetUser);
    }

    /// <summary>
    /// Determines whether one user is following another.
    /// </summary>
    /// <param name="currentUserName">Username of the querying user.</param>
    /// <param name="targetUserName">Username of the target user.</param>
    /// <returns>
    /// <c>true</c> if the current user is following the target user;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if either user does not exist or if the same user is provided for both parameters.
    /// </exception>
    public async Task<bool> IsFollowing(string currentUserName, string targetUserName)
    {
        AuthorDTO? currentUser = await _authorRepository.GetAuthorByName(currentUserName);
        if (currentUser == null)
        {
            throw new InvalidOperationException($"user with username: '{currentUserName}' doesn't exist");
        }

        AuthorDTO? targetUser = await _authorRepository.GetAuthorByName(targetUserName);
        if (targetUser == null)
        {
            throw new InvalidOperationException($"user with username: '{targetUserName}' doesn't exist");
        }

        if (currentUser.AuthorId == targetUser.AuthorId)
        {
            throw new InvalidOperationException($"You cannot follow yourself");
        }

        bool IsFollowing = await _authorRepository.IsFollowing(currentUser, targetUser);
        return IsFollowing;
    }

    /// <summary>
    /// Retrieves all authors followed by a given user.
    /// </summary>
    /// <param name="userName">Username of the author.</param>
    /// <returns>
    /// A list of authors that the user is following.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user does not exist.
    /// </exception>
    public async Task<List<AuthorDTO>> GetFollowing(string userName)
    {
        AuthorDTO? author = await _authorRepository.GetAuthorByName(userName);
        if (author == null)
        {
            throw new InvalidOperationException($"user with username: '{userName}' doesn't exist");
        }

        List<AuthorDTO> following = await _authorRepository.GetFollowing(author);
        return following;
    }

    /// <summary>
    /// Deletes an author account.
    /// </summary>
    /// <param name="userName">Username of the author to delete.</param>
    /// <returns>
    /// An <see cref="IdentityResult"/> indicating whether the deletion succeeded.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user does not exist.
    /// </exception>
    public async Task<IdentityResult> DeleteAuthor(string userName)
    {
        AuthorDTO? user = await _authorRepository.GetAuthorByName(userName);
        if (user == null)
        {
            throw new InvalidOperationException($"user with username: '{userName}' doesn't exist");
        }
        //handle everything in cheep Repository prior deleting the author
        await _cheepRepository.DeleteSavedCheeps(userName);
        await _cheepRepository.DeleteCheeps(userName);

        return await _authorRepository.DeleteAuthor(user);

    }
}