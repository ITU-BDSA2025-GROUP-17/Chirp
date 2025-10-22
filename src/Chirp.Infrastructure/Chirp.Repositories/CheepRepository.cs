using Microsoft.EntityFrameworkCore;
namespace Chirp.Repositories;

using System.Collections.ObjectModel;
using Core;

public class CheepRepository : ICheepRepository
{
    //access the database
    private readonly CheepDBContext _dbContext;
    public CheepRepository(CheepDBContext context)
    {
        _dbContext = context;
    }

    public async Task CreateCheep(CheepDTO cheep)
    {
        if (cheep.Author == null) throw new NullReferenceException("Author is null");
        if (cheep.Author.AuthorId == null) throw new NullReferenceException("AuthorId is null");
        Cheep newCheep = new()
        {
            AuthorId = (long)cheep.Author.AuthorId,
            Text = cheep.Text,
            TimeStamp = cheep.TimeStamp
        };
        await _dbContext.Messages.AddAsync(newCheep); // does not write to the database!
        await _dbContext.SaveChangesAsync(); // persist the changes in the database
    }

    public async Task<List<CheepDTO>> ReadCheeps(string? user, int offset, int count)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = from message in _dbContext.Messages
                    where message.Author.UserName == user || user == null
                    orderby message.TimeStamp descending
                    select new CheepDTO{
                            Text = message.Text, 
                            TimeStamp = message.TimeStamp, 
                            CheepId = message.CheepId,
                            Author = new() {
                                AuthorId = message.Author.AuthorId,
                                Name = message.Author.UserName,
                                Email = message.Author.Email
                            }
                    };
        
        // Execute the query and store the results
        var result = await query.Skip(offset).Take(count).ToListAsync();
        return result;
    }

    public async Task UpdateCheep(CheepDTO cheep)
    {
        var query = from message in _dbContext.Messages
            where message.CheepId == cheep.CheepId
            select message;

        var result = await query.ToListAsync();
        if (result.Count < 1)
        {
            result[0].Text = cheep.Text;
            _dbContext.SaveChanges();
        }
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
            Name = result.UserName,
            Email = result.Email,
            Messages = result.Messages.Select(m => new CheepDTO {
                CheepId = m.CheepId,
                Text = m.Text,
                TimeStamp = m.TimeStamp,
                Author = new AuthorDTO {
                    Name = result.UserName,
                    Email = result.Email
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
            Name = result.UserName,
            Email = result.Email,
            Messages = result.Messages.Select(m => new CheepDTO {
                CheepId = m.CheepId,
                Text = m.Text,
                TimeStamp = m.TimeStamp,
                Author = new AuthorDTO {
                    Name = result.UserName,
                    Email = result.Email
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