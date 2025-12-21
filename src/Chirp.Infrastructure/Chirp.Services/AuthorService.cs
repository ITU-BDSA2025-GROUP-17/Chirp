namespace Chirp.Service;

using Microsoft.AspNetCore.Identity;
using Repositories;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }


    public async Task<AuthorDTO?> getAuthorByName(string userName) { }
    public async Task<AuthorDTO?> getAuthorByEmail(string email) { }
    public async Task CreateAuthor(string userName, string email) { }
    public async Task FollowUser(string currentUserName, string targetUserName) { }
    public async Task UnfollowUser(string currentUserName, string targetUserName) { }
    public async Task<bool> IsFollowing(string currentUserName, string targetUserName) { }
    public async Task<List<AuthorDTO>> GetFollowing(string userName) { }
    public async Task<IdentityResult> DeleteAuthor(string userName) { }
}