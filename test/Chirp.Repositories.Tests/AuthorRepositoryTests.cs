using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Chirp.Repositories.Tests;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Chirp.Core;
using Chirp.Repositories;
//using Services;
using Chirp.Web;

public class AuthorRepositoryTests
{
    private IAuthorRepository? _authorRepo;
    private CheepDBContext? _db;

    private void SetUpAuthorRepositoryTests()
    {
        Utility.resetUsernames();
        // Use an in-memory SQLite database for testing
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var services = new ServiceCollection();
        services.AddDbContext<CheepDBContext>(options =>
            options.UseSqlite(connection));

        services.AddScoped<ICheepRepository, CheepRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();

        var provider = services.BuildServiceProvider();

        _db = provider.GetRequiredService<CheepDBContext>();
        _db.Database.EnsureCreated(); // create tables in memory

        _authorRepo = provider.GetService<IAuthorRepository>();
    }


    [Fact]
    public async Task CreateAuthorTest()
    {
        SetUpAuthorRepositoryTests();
        if (_authorRepo == null) throw new NullReferenceException("_authorRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        for (var i = 0; i < 1000; i++)
        {
            var authorName = Utility.RandomString(new Random().Next(1, 25));
            var authorEmail = authorName + "@MPGA.gov";

            try
            {
                await _authorRepo!.CreateAuthor(authorName, authorEmail);
            }

            catch (DbUpdateException e)
            {
                _ = e;
                // Expected, as usernames will sometimes overlap
                // When this happens, the unique requirement of Author.Name is violated
                continue;
            }

            var authors = _db.Users;
            var success = false;
            foreach (var author in authors)
            {
                if (author.UserName == authorName)
                {
                    success = true;
                }
            }
            Assert.True(success);
        }
    }

    [Fact]
    public async Task ReadAuthorFromNameTest()
    {
        SetUpAuthorRepositoryTests();
        if (_authorRepo == null) throw new NullReferenceException("_authorRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        var testAuthor = Utility.RandomTestUser(true);
        _db.Users.AddRange(new List<Author> { testAuthor });
        await _db.SaveChangesAsync();

        var result = await _authorRepo!.GetAuthorByName(testAuthor.UserName!);
        Assert.NotNull(result);
        Assert.Equal(testAuthor.UserName, result.Name);
    }

    [Fact]
    public async Task ReadAuthorFromEmailTest()
    {
        SetUpAuthorRepositoryTests();
        if (_authorRepo == null) throw new NullReferenceException("_authorRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        var testAuthor = Utility.RandomTestUser(true);
        _db.Users.AddRange(new List<Author> { testAuthor });
        await _db.SaveChangesAsync();

        var result = await _authorRepo!.GetAuthorByEmail(testAuthor.Email!);
        Assert.NotNull(result);
        Assert.Equal(testAuthor.Email, result.Email);
    }



    //follow test
    [Fact]
    public async Task FollowTest()
    {
        SetUpAuthorRepositoryTests();
        if (_authorRepo == null) throw new NullReferenceException("_authorRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        //make author
        var testAuthor = Utility.RandomTestUser(true);
        var testFollow = Utility.RandomTestUser(true);
        _db.Users.AddRange(new List<Author> { testAuthor, testFollow });
        await _db.SaveChangesAsync();

        var testAuthorDto = new AuthorDTO
        {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };
        // make follow author, that author should follow
        var testFollowAuthDto = new AuthorDTO
        {
            AuthorId = testFollow.Id,
            Name = testFollow.UserName!,
            Email = testFollow.Email!
        };

        await _authorRepo.Follow(testAuthorDto, testFollowAuthDto);

        var followList = await _authorRepo.GetFollowing(testAuthorDto);

        bool check = false;
        foreach (var following in followList)
        {
            if (following.AuthorId == testFollowAuthDto.AuthorId) check = true; break;
        }

        Assert.True(check);
    }

    //unfollow test
    [Fact]
    public async Task UnFollowTest()
    {
        SetUpAuthorRepositoryTests();
        if (_authorRepo == null) throw new NullReferenceException("_authorRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        //author
        var testAuthor = Utility.RandomTestUser(true);
        var testFollow = Utility.RandomTestUser(true);
        _db.Users.AddRange(new List<Author> { testAuthor, testFollow });
        await _db.SaveChangesAsync();

        var testAuthorDto = new AuthorDTO
        {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };
        //follow author, that author should follow
        var testFollowAuthDto = new AuthorDTO
        {
            AuthorId = testFollow.Id,
            Name = testFollow.UserName!,
            Email = testFollow.Email!
        };

        //testAuthor follow testFollowAuth
        await _authorRepo.Follow(testAuthorDto, testFollowAuthDto);

        //unfollow
        await _authorRepo.UnFollow(testAuthorDto, testFollowAuthDto);

        var followList = await _authorRepo.GetFollowing(testAuthorDto);

        bool check = false;
        foreach (var following in followList)
        {
            if (following.AuthorId == testFollowAuthDto.AuthorId) check = true; break;
        }
        Assert.False(check);
    }

    [Fact]
    public async Task IsFollowingTest()
    {
        SetUpAuthorRepositoryTests();
        if (_authorRepo == null) throw new NullReferenceException("_authorRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        //make authors
        var testAuthor = Utility.RandomTestUser(true);
        var testFollow = Utility.RandomTestUser(true);
        _db.Users.AddRange(new List<Author> { testAuthor, testFollow });
        await _db.SaveChangesAsync();

        var testAuthorDto = new AuthorDTO
        {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };
        var testFollowAuthDto = new AuthorDTO
        {
            AuthorId = testFollow.Id,
            Name = testFollow.UserName!,
            Email = testFollow.Email!
        };

        //before following
        var isFollowingBefore = await _authorRepo.IsFollowing(testAuthorDto, testFollowAuthDto);
        Assert.False(isFollowingBefore);

        //testAuthor follow testFollowAuth
        await _authorRepo.Follow(testAuthorDto, testFollowAuthDto);

        //after following
        var isFollowingAfter = await _authorRepo.IsFollowing(testAuthorDto, testFollowAuthDto);
        Assert.True(isFollowingAfter);
    }

    [Fact]
    public async Task GetFollowingTest()
    {
        SetUpAuthorRepositoryTests();
        if (_authorRepo == null) throw new NullReferenceException("_authorRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        //make authors
        var testAuthor = Utility.RandomTestUser(true);
        var testFollow1 = Utility.RandomTestUser(true);
        var testFollow2 = Utility.RandomTestUser(true);
        _db.Users.AddRange(new List<Author> { testAuthor, testFollow1, testFollow2 });
        await _db.SaveChangesAsync();

        var testAuthorDto = new AuthorDTO
        {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };
        var testFollow1Dto = new AuthorDTO
        {
            AuthorId = testFollow1.Id,
            Name = testFollow1.UserName!,
            Email = testFollow1.Email!
        };
        var testFollow2Dto = new AuthorDTO
        {
            AuthorId = testFollow2.Id,
            Name = testFollow2.UserName!,
            Email = testFollow2.Email!
        };

        //following list should be empty
        var followingListBefore = await _authorRepo.GetFollowing(testAuthorDto);
        Assert.Empty(followingListBefore);

        //testAuthor follow two different users users
        await _authorRepo.Follow(testAuthorDto, testFollow1Dto);
        await _authorRepo.Follow(testAuthorDto, testFollow2Dto);

        //test following
        var followingListAfter = await _authorRepo.GetFollowing(testAuthorDto);
        Assert.Equal(2, followingListAfter.Count);

        Assert.Contains(followingListAfter, f => f.AuthorId == testFollow1Dto.AuthorId);
        Assert.Contains(followingListAfter, f => f.AuthorId == testFollow2Dto.AuthorId);
    }

    [Fact]
    public async Task DeleteAuthorTest()
    {
        SetUpAuthorRepositoryTests();
        if (_authorRepo == null) throw new NullReferenceException("_authorRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        //make authors
        var testAuthor = Utility.RandomTestUser(true);
        var testFollow = Utility.RandomTestUser(true);
        _db.Users.AddRange(new List<Author> { testAuthor, testFollow });
        await _db.SaveChangesAsync();

        var testAuthorDto = new AuthorDTO
        {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };
        var testFollowAuthDto = new AuthorDTO
        {
            AuthorId = testFollow.Id,
            Name = testFollow.UserName!,
            Email = testFollow.Email!
        };

        //follow
        await _authorRepo.Follow(testAuthorDto, testFollowAuthDto);

        //delete author
        var result = await _authorRepo.DeleteAuthor(testAuthorDto);
        Assert.True(result.Succeeded);

        //check author is deleted in database
        var deletedAuthor = await _db.Users.FirstOrDefaultAsync(a => a.Id == testAuthor.Id);
        Assert.Null(deletedAuthor);

        //follow is also deleted
        var followRelationship = await _db.Follows.FirstOrDefaultAsync(f => f.FollowerId == testAuthor.Id);
        Assert.Null(followRelationship);
    }
}
