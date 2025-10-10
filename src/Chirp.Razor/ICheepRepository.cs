namespace Chirp.Razor.wwwroot;

public interface ICheepRepository
{
    Task CreateCheep(CheepDTO cheep);
    Task<List<CheepDTO>> ReadCheeps(string? user, int offset, int count);
    Task UpdateCheep(CheepDTO cheep);
    Task<AuthorDTO?> GetAuthorByName(string authorName);
    Task<AuthorDTO?> GetAuthorByEmail(string authorEmail);
    Task CreateAuthor(AuthorDTO author);
    
    
}