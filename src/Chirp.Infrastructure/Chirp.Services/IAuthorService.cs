namespace Chirp.Services;

using Microsoft.AspNetCore.Identity;
using Repositories;

public interface IAuthorService
{
    Task<AuthorDTO?> GetAuthorByName(string userName);
    Task<AuthorDTO?> GetAuthorByEmail(string email);
    Task CreateAuthor(string userName, string email);
    Task FollowUser(string currentUserName, string targetUserName);
    Task UnfollowUser(string currentUserName, string targetUserName);
    Task<bool> IsFollowing(string currentUserName, string targetUserName);
    Task<List<AuthorDTO>> GetFollowing(string userName);
    Task<IdentityResult> DeleteAuthor(string userName);
}