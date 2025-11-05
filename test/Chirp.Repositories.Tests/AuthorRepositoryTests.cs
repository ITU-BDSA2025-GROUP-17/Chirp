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
        if(_db == null) throw new NullReferenceException("_db is null");
        
        for (var i = 0; i < 1000; i++)
        {
            var authorName = Utility.RandomString(new Random().Next(1,25));
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
        if(_db == null) throw new NullReferenceException("_db is null");
        
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
        if(_db == null) throw new NullReferenceException("_db is null");

        var testAuthor = Utility.RandomTestUser(true);
        _db.Users.AddRange(new List<Author> { testAuthor });
        await _db.SaveChangesAsync();

        var result = await _authorRepo!.GetAuthorByEmail(testAuthor.Email!);
        Assert.NotNull(result);
        Assert.Equal(testAuthor.Email, result.Email);
    }
}
