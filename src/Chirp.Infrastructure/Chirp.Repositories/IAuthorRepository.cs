namespace Chirp.Repositories;
using Core;

public interface IAuthorRepository
{
    Task<AuthorDTO?> GetAuthorByName(string authorName);
    Task<AuthorDTO?> GetAuthorByEmail(string authorEmail);
    Task CreateAuthor(string authorName,string authorEmail);
    Task Follow(AuthorDTO userAuthor, AuthorDTO followAuthor);
    Task<Boolean> IsFollowing(AuthorDTO userAuthor, AuthorDTO followAuthor);
    Task<List<AuthorDTO>> GetFollowing(AuthorDTO userAuthor);



}