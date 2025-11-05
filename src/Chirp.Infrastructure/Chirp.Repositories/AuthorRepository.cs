using Microsoft.EntityFrameworkCore;
namespace Chirp.Repositories;

using Core;

public class AuthorRepository : IAuthorRepository
{
    //access the database
    private readonly CheepDBContext _dbContext;
    public AuthorRepository(CheepDBContext context)
    {
        _dbContext = context;

    }
    public async Task<AuthorDTO?> GetAuthorByName(string authorName)
    {
        var query = from author in _dbContext.Users
            where author.UserName == authorName
            select author
        ;

        var result = await query.FirstOrDefaultAsync();
        if (result == null) throw new NullReferenceException("resulting author is null");
        
        return new AuthorDTO
        {
            Name = result.UserName!,
            Email = result.Email!,
            Messages = result.Cheeps!.Select(m => new CheepDTO {
                CheepId = m.CheepId,
                Text = m.Text,
                TimeStamp = m.TimeStamp,
                Author = new AuthorDTO {
                    Name = result.UserName!,
                    Email = result.Email!
                }
            }).ToList()
        };
    }

    public async Task<AuthorDTO?> GetAuthorByEmail(string authorEmail)
    {
        var query = from author in _dbContext.Users
            where author.Email == authorEmail
            select author
        ;

        var result = await query.FirstOrDefaultAsync();
        if (result == null) throw new NullReferenceException("resulting author is null");
        
        return new AuthorDTO
        {
            Name = result.UserName!,
            Email = result.Email!,
            Messages = result.Cheeps!.Select(m => new CheepDTO {
                CheepId = m.CheepId,
                Text = m.Text,
                TimeStamp = m.TimeStamp,
                Author = new AuthorDTO {
                    Name = result.UserName!,
                    Email = result.Email!
                }
            }).ToList()
        };
    }

    public async Task CreateAuthor(string authorName, string authorEmail)
    {
        var newAuthor = new Author()
        {
            UserName = authorName,
            Email = authorEmail,
        };
        await _dbContext.Users.AddAsync(newAuthor); // does not write to the database!
        await _dbContext.SaveChangesAsync(); // persist the changes in the database
    }
}