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
                .Include(a => a.Cheeps)
                    where author.UserName == authorName
                    select author;

        var result = await query.FirstOrDefaultAsync();
        if (result == null) throw new NullReferenceException("resulting author is null");

        return new AuthorDTO
        {
            Name = result.UserName!,
            AuthorId = result.Id,
            Email = result.Email!,
            Messages = result.Cheeps!.Select(m => new CheepDTO
            {
                CheepId = m.CheepId,
                Text = m.Text,
                TimeStamp = m.TimeStamp,
                Author = new AuthorDTO
                {
                    Name = result.UserName!,
                    AuthorId = result.Id,
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

    public async Task Follow(AuthorDTO userAuthor, AuthorDTO followAuthor)
    {
        if (userAuthor == null) throw new NullReferenceException("userAuthor is null");
        if (followAuthor == null) throw new NullReferenceException("followAuthor is null");

        if (userAuthor.AuthorId == followAuthor.AuthorId) throw new InvalidOperationException("userAuthor cannot follow them self");
        if (userAuthor.Following == null)
        {
            userAuthor.Following = new List<AuthorDTO>();
        }

        // find userAuthor
        var userQuery = from author in _dbContext.Users
                .Include(a => a.Following)
                        where author.Id == userAuthor.AuthorId
                        select author;

        var user = await userQuery.FirstOrDefaultAsync();
        if (user == null) throw new NullReferenceException("resulting author is null");


        // find followAuthor
        var followQuery = from author in _dbContext.Users
                .Include(a => a.Following)
                          where author.Id == followAuthor.AuthorId
                          select author;

        var following = await followQuery.FirstOrDefaultAsync();
        if (following == null) throw new NullReferenceException("resulting author is null");

        if (user.Following == null)
        {
            user.Following = new List<Follow>();
        }

        user.Following.Add(
            new Follow()
            {
                FollowerId = user.Id,
                Follower = user,
                FollowingId = following.Id,
                Following = following,
            }
        );
        await _dbContext.SaveChangesAsync();
    }

    // Is user following followAUt
    public async Task<Boolean> IsFollowing(AuthorDTO userAuthor, AuthorDTO followAuthor)
    {
        var userFollowings = await GetFollowing(userAuthor);
        foreach (var user in userFollowings)
        {
            if (user.AuthorId == followAuthor.AuthorId) return true;
        }

        return false;

    }

    // returns an empty list of authordtos if the user does not follow anyone / or list is null
    public async Task<List<AuthorDTO>> GetFollowing(AuthorDTO userAuthor)
    {
        var query = from author in _dbContext.Users
            .Include(a => a.Following)
                .ThenInclude(f => f.Following)
                    where author.Id == userAuthor.AuthorId
                    select author;

        var result = await query.FirstOrDefaultAsync();

        if (result == null) throw new NullReferenceException("resulting author is null");
        if (result.Following == null || result.Following.Count == 0)
        {
            return new List<AuthorDTO>();
        }

        // return en liste af dto's
        var followings = new List<AuthorDTO>();
        foreach (var follow in result.Following!)
        {
            followings.Add(
                new AuthorDTO
                {
                    Name = follow.Following!.UserName!,
                    Email = follow.Following!.Email!,
                    AuthorId = follow.Following!.Id
                }
            );
        }

        return followings;

    }

}