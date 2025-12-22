using Microsoft.EntityFrameworkCore;
namespace Chirp.Repositories;

using System.Runtime.CompilerServices;
using Core;

public class CheepRepository : ICheepRepository
{
    //access the database
    private readonly CheepDBContext _dbContext;
    private readonly IAuthorRepository _authorRepository;
    public CheepRepository(CheepDBContext context)
    {
        _dbContext = context;
        _authorRepository = new AuthorRepository(_dbContext);
    }

    public async Task CreateCheep(CheepDTO cheep)
    {
        if (cheep.Author == null) throw new NullReferenceException("Author is null");
        if (cheep.Text.Length > 160) return; //if cheep length is more than 160 chars, nothing happens
        Cheep newCheep = new()
        {
            Author = await _dbContext.Users.FindAsync(cheep.Author.AuthorId),
            Text = cheep.Text,
            TimeStamp = cheep.TimeStamp
        };
        await _dbContext.Cheeps.AddAsync(newCheep); // does not write to the database!
        await _dbContext.SaveChangesAsync(); // persist the changes in the database
    }

    public async Task SaveCheep(AuthorDTO user, CheepDTO cheep)
    {
        if (cheep.CheepId == null) throw new NullReferenceException("Cheep ID is null!");
        if (cheep.Author == null) throw new NullReferenceException("Cheep author is null!");
        SavedCheep newCheep = new()
        {
            Saver = await _dbContext.Users.FindAsync(user.AuthorId),
            CheepId = (long)cheep.CheepId,
            TimeStamp = DateTime.Now
        };
        await _dbContext.SavedCheeps.AddAsync(newCheep); // does not write to the database!
        await _dbContext.SaveChangesAsync(); // persist the changes in the database
    }

    public async Task RemoveSavedCheep(AuthorDTO user, CheepDTO cheep)
    {
        if (cheep.CheepId == null) throw new NullReferenceException("Cheep ID is null!");

        var savedCheep = await _dbContext.SavedCheeps
            .FirstOrDefaultAsync(
                s => s.Saver!.Id == user.AuthorId
                && s.CheepId == cheep.CheepId
            );

        if (savedCheep == null) return;

        _dbContext.SavedCheeps.Remove(savedCheep);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<CheepDTO>> ReadCheeps(string? user, int offset, int count)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = from message in _dbContext.Cheeps
                    where message.Author!.UserName == user || user == null
                    orderby message.TimeStamp descending
                    select new CheepDTO
                    {
                        Text = message.Text,
                        TimeStamp = message.TimeStamp,
                        CheepId = message.CheepId,
                        Author = new()
                        {
                            AuthorId = message.Author!.Id,
                            Name = message.Author.UserName!,
                            Email = message.Author.Email!
                        }
                    };

        // Execute the query and store the results
        return await query.Skip(offset).Take(count).ToListAsync();
    }

    public async Task<List<CheepDTO>> ReadSavedCheeps(int authorId, int offset, int count)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = from save in _dbContext.SavedCheeps
                    join cheep in _dbContext.Cheeps on save.CheepId equals cheep.CheepId
                    where save.Saver!.Id == authorId
                    orderby save.TimeStamp descending
                    select new CheepDTO
                    {
                        Text = cheep.Text,
                        TimeStamp = cheep.TimeStamp,
                        CheepId = cheep.CheepId,
                        Author = new()
                        {
                            AuthorId = cheep.Author!.Id,
                            Name = cheep.Author.UserName!,
                            Email = cheep.Author.Email!
                        }
                    };

        // Execute the query and return the results
        return await query.Skip(offset).Take(count).ToListAsync();
    }

    public async Task<List<CheepDTO>> ReadCheepsWithSearch(string? user, string? search, int offset, int count)
    {
        if (search == null)
        {
            search = "";
        }
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = from message in _dbContext.Cheeps
                    where (message.Author!.UserName == user || user == null) && message.Text.Contains(search)
                    orderby message.TimeStamp descending
                    select new CheepDTO
                    {
                        Text = message.Text,
                        TimeStamp = message.TimeStamp,
                        CheepId = message.CheepId,
                        Author = new()
                        {
                            AuthorId = message.Author!.Id,
                            Name = message.Author.UserName!,
                            Email = message.Author.Email!
                        }
                    };

        // Execute the query and store the results
        var result = await query.Skip(offset).Take(count).ToListAsync();
        return result;
    }

    public async Task UpdateCheep(CheepDTO cheep)
    {
        var query = from message in _dbContext.Cheeps
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
                .Include(a => a.Cheeps)
                    where author.UserName == authorName
                    select author;

        var result = await query.FirstOrDefaultAsync();
        if (result == null) throw new NullReferenceException("resulting author is null");

        return new AuthorDTO
        {
            Name = result.UserName!,
            Email = result.Email!,
            Messages = result.Cheeps!.Select(m => new CheepDTO
            {
                CheepId = m.CheepId,
                Text = m.Text,
                TimeStamp = m.TimeStamp,
                Author = new AuthorDTO
                {
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
            Messages = result.Cheeps!.Select(m => new CheepDTO
            {
                CheepId = m.CheepId,
                Text = m.Text,
                TimeStamp = m.TimeStamp,
                Author = new AuthorDTO
                {
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


    public async Task<List<CheepDTO>> ReadCheepsFromFollowers(string user, int offset, int count)
    {
        var author = await _authorRepository.GetAuthorByName(user!); // -------------------------------------------------------ch
        if (author == null) { throw new Exception("Author " + user + " not found!"); }
        var following = await _authorRepository.GetFollowing(author);

        var users = new List<string> { user };
        foreach (var follow in following)
        {
            users.Add(follow.Name);
        }

        var query = from message in _dbContext.Cheeps

                    where users.Contains(message.Author!.UserName!)
                    orderby message.TimeStamp descending
                    select new CheepDTO
                    {
                        Text = message.Text,
                        TimeStamp = message.TimeStamp,
                        CheepId = message.CheepId,
                        Author = new()
                        {
                            AuthorId = message.Author!.Id,
                            Name = message.Author.UserName!,
                            Email = message.Author.Email!
                        }
                    };


        // Execute the query and store the results
        var result = await query.Skip(offset).Take(count).ToListAsync();
        return result;
    }

    public async Task<CheepDTO?> GetCheepById(long cheepId)
    {
        var cheep = await _dbContext.Cheeps.Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.CheepId == cheepId);

        if (cheep == null) return null;

        return new CheepDTO
        {
            Text = cheep.Text,
            TimeStamp = cheep.TimeStamp,
            CheepId = cheep.CheepId,
            Author = new()
            {
                AuthorId = cheep.Author!.Id,
                Name = cheep.Author.UserName!,
                Email = cheep.Author.Email!
            }
        };
    }

    public async Task DeleteSavedCheeps(string userName)
    {
        var query = from author in _dbContext.Users
            .Include(a => a.SavedCheeps)
                    where author.UserName == userName
                    select author;

        var user = await query.FirstOrDefaultAsync();
        if (user == null) throw new NullReferenceException("resulting author is null");
        _dbContext.SavedCheeps.RemoveRange(user.SavedCheeps!);

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCheeps(string userName)
    {
        var query = from author in _dbContext.Users
            .Include(a => a.Cheeps)
                    where author.UserName == userName
                    select author;

        var user = await query.FirstOrDefaultAsync();
        if (user == null) throw new NullReferenceException("resulting author is null");

        if (user.Cheeps == null) return;

        var cheepIds = user.Cheeps.Select(c => c.CheepId).ToHashSet();

        // Delete all saved entries for these cheeps first
        var savedCheeps = await _dbContext.SavedCheeps
            .Where(s => cheepIds.Contains(s.CheepId))
            .ToListAsync();

        _dbContext.SavedCheeps.RemoveRange(savedCheeps);

        _dbContext.Cheeps.RemoveRange(user.Cheeps!);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsSaved(AuthorDTO user, CheepDTO cheep)
    {
        return await _dbContext.SavedCheeps.AnyAsync(
            save => save.Saver!.Id == user.AuthorId
            && save.CheepId == cheep.CheepId
        );
    }
}