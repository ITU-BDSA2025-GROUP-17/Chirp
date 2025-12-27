namespace Chirp.Services;

using Microsoft.AspNetCore.Identity;
using Repositories;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }


    public async Task<AuthorDTO?> GetAuthorByName(string userName)
    {
        AuthorDTO? author = await _authorRepository.GetAuthorByName(userName);
        return author;
    }

    public async Task<AuthorDTO?> GetAuthorByEmail(string email)
    {
        AuthorDTO? author = await _authorRepository.GetAuthorByEmail(email);
        return author;
    }
    public async Task CreateAuthor(string userName, string email)
    {
        await _authorRepository.CreateAuthor(userName, email);
    }
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

        if (currentUser.AuthorId == targetUser.AuthorId)
        {
            throw new InvalidOperationException($"You cannot follow yourself");
        }

        await _authorRepository.UnFollow(currentUser, targetUser);
    }
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
    public async Task<IdentityResult> DeleteAuthor(string userName)
    {
        AuthorDTO? user = await _authorRepository.GetAuthorByName(userName);
        if (user == null)
        {
            throw new InvalidOperationException($"user with username: '{userName}' doesn't exist");
        }
        IdentityResult result = await _authorRepository.DeleteAuthor(user);
        //identityResult returns if its succeded, and if not, a list with the errors. 
        return result;
    }
}