namespace Chirp.Services;

public interface IAuthorService
{
    Task<AuthorDTO?> getAuthorByName(string userName);
    Task<AuthorDTO?> getAuthorByEmail(string email);
    Task CreateAuthor(string userName, string email);
    Task FollowUser(string currentUserName, string targetUserName);
    Task UnfollowUser(string currentUserName, string targetUserName);
    Task<bool> IsFollowing(string currentUserName, string targetUserName);
    Task<List<AuthorDTO>> GetFollowing(string userName);
    Task<IdentityResult> DeleteAuthor(string userName);
}