namespace Chirp.Repositories;
using Core;

public interface IAuthorRepository
{
    Task<AuthorDTO?> GetAuthorByName(string authorName);
    Task<AuthorDTO?> GetAuthorByEmail(string authorEmail);
    Task CreateAuthor(string authorName,string authorEmail);
}