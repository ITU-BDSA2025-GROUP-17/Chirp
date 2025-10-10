using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.wwwroot;

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
        Cheep newCheep = new()
        {
            AuthorId = cheep.Author.AuthorId,
            Text = cheep.Text,
            TimeStamp = cheep.TimeStamp,
            Author = cheep.Author
        };
        await _dbContext.Messages.AddAsync(newCheep); // does not write to the database!
        await _dbContext.SaveChangesAsync(); // persist the changes in the database
    }

    public async Task<List<CheepDTO>> ReadCheeps(string? user, int offset, int count)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = from message in _dbContext.Messages
                    where message.Author.Name == user || user == null
                    orderby message.TimeStamp descending
                    select new CheepDTO{
                            Author = message.Author, 
                            Text = message.Text, 
                            TimeStamp = message.TimeStamp, 
                            CheepId = message.CheepId
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
            where author.Name == authorName
            select new AuthorDTO{
                Name = author.Name,
                Email = author.Email,
                Messages = author.Messages
            };
        
        var result = await query.FirstOrDefaultAsync();
        return result;
    }

    public async Task<AuthorDTO?> GetAuthorByEmail(string authorEmail)
    {
        var query = from author in _dbContext.Users
            where author.Email == authorEmail
            select new AuthorDTO{
                Name = author.Name,
                Email = author.Email,
                Messages = author.Messages
            };
        
        var result = await query.FirstOrDefaultAsync();
        return result;
    }

    public async Task CreateAuthor(string authorName, string authorEmail)
    {
        var newAuthor = new Author()
        {
            Name = authorName,
            Email = authorEmail,
        };
        await _dbContext.Users.AddAsync(newAuthor); // does not write to the database!
        await _dbContext.SaveChangesAsync(); // persist the changes in the database
    }
}