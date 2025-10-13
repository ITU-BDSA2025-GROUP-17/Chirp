using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Chirp.DB.Tests;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Chirp.Core;
using Chirp.Repositories;
//using Services;
using Chirp.Web;

public class CheepRepositoryTests
{
    private ICheepRepository? _repo;
    private CheepDBContext? _db;
	
	private HashSet<string>? _usedAuthorNames;

    private void SetUpCheepRepositoryTests()
    {
        // Use an in-memory SQLite database for testing
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var services = new ServiceCollection();
        services.AddDbContext<CheepDBContext>(options =>
            options.UseSqlite(connection));

        services.AddScoped<ICheepRepository, CheepRepository>();

        var provider = services.BuildServiceProvider();

        _db = provider.GetRequiredService<CheepDBContext>();
        _db.Database.EnsureCreated(); // create tables in memory

        _repo = provider.GetRequiredService<ICheepRepository>();

		_usedAuthorNames = new HashSet<string>();
    }

    private string RandomString(int length)
    {
        var generatedString = new char[length];
        for (var i = 0; i<length ; i++)
        {
            generatedString[i] = (char) (new Random().Next(33, 127));
        }
        return new string(generatedString);
    }

    private Author RandomTestUser()
    {
        if (_usedAuthorNames == null) throw new NullReferenceException("_usedAuthorNames is null");
		var username = RandomString(new Random().Next(1, 25));
		while(_usedAuthorNames.Contains(username)) {
			username = RandomString(new Random().Next(1, 25));
		}

        _usedAuthorNames.Add(username);

        return new Author() {
            Name = username, 
            Email = username+"@hotmail.com", 
            Messages = new List<Cheep>()
        };
    }

    private Cheep RandomTestCheep(Author author, int length, double? time)
    {
        var actualTime = time == null ? new Random().NextDouble() : (double)time;
        var message = RandomString(length);
        return new Cheep {
            Author = author,
            AuthorId = author.AuthorId, 
            Text = message, 
            TimeStamp = DateTime.UnixEpoch.AddSeconds(actualTime),
        };
    }

    [Fact]
    public async Task ReadCheepTest()
    {
        SetUpCheepRepositoryTests();
        if (_repo == null) throw new NullReferenceException("_repo is null");
        if(_db == null) throw new NullReferenceException("_db is null");
        
        for(var i = 0; i < 1000; i++) {
            // Arrange
            var testAuthor = RandomTestUser();
			var cheep = RandomTestCheep(testAuthor, 150, i);
            testAuthor.Messages.Add(cheep);

            _db.Users.AddRange(new List<Author> { testAuthor });
            _db.Messages.AddRange(new List<Cheep> { cheep });
            await _db.SaveChangesAsync();
            

            // Act
            var cheeps = await _repo.ReadCheeps(null, 0, 1);

            // Assert
            Assert.Equal(cheeps[0].Author.Name, testAuthor.Name);
            Assert.Equal(cheeps[0].Text, cheep.Text);
        }
    }

    [Fact]
    public async Task WriteCheepTest()
    {
        SetUpCheepRepositoryTests();
        if (_repo == null) throw new NullReferenceException("_repo is null");
        if(_db == null) throw new NullReferenceException("_db is null");
        
        var testAuthor = RandomTestUser();
        var testAuthorDto = new AuthorDTO {
            AuthorId = 1,
            Name = testAuthor.Name,
            Email = testAuthor.Email
        };
        _db.Users.AddRange(new List<Author> { testAuthor });
        await _db.SaveChangesAsync();
        for(var i = 0; i < 1000; i++) {
            var testCheep = RandomTestCheep(testAuthor, 150, i);
            var testCheepDto = new CheepDTO {
                Text = testCheep.Text,  
                Author = testAuthorDto,  
                TimeStamp = testCheep.TimeStamp
            };

            await _repo.CreateCheep(testCheepDto);

            var success = false;
            foreach (var cheep in _db.Messages)
            {
                if(cheep.Text == testCheep.Text)
                {
                    success = true;
                    break;
                }
            }
            Assert.True(success);
        }
    }

    /*[Fact]
    public async Task WriteCheepExceedingLimitTest()
    {
        SetUpCheepRepositoryTests();
        if (_repo == null) throw new NullReferenceException("_repo is null");
        if(_db == null) throw new NullReferenceException("_db is null");
        
        Author testAuthor = RandomTestUser();
        Cheep cheepExceedingLimit = RandomTestCheep(testAuthor, 200, null);

        testAuthor.Messages.Add(cheepExceedingLimit);

        _db.Users.AddRange(new List<Author> { testAuthor });
        _db.Messages.AddRange(new List<Cheep> { cheepExceedingLimit });
        await _db.SaveChangesAsync();
    }*/

    [Fact]
    public async Task CreateAuthorTest()
    {
        SetUpCheepRepositoryTests();
        if (_repo == null) throw new NullReferenceException("_repo is null");
        if(_db == null) throw new NullReferenceException("_db is null");
        
        for (var i = 0; i < 1000; i++)
        {
            var authorName = RandomString(new Random().Next(1,25));
            var authorEmail = authorName + "@MPGA.gov";

            try
            {
                await _repo.CreateAuthor(authorName, authorEmail);
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
                if (author.Name == authorName)
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
        SetUpCheepRepositoryTests();
        if (_repo == null) throw new NullReferenceException("_repo is null");
        if(_db == null) throw new NullReferenceException("_db is null");
        
        var testAuthor = RandomTestUser();
        _db.Users.AddRange(new List<Author> { testAuthor });
        await _db.SaveChangesAsync();

        var result = await _repo.GetAuthorByName(testAuthor.Name);
        Assert.NotNull(result);
        Assert.Equal(testAuthor.Name, result.Name);
    }

    [Fact]
    public async Task ReadAuthorFromEmailTest()
    {
        SetUpCheepRepositoryTests();
        if (_repo == null) throw new NullReferenceException("_repo is null");
        if(_db == null) throw new NullReferenceException("_db is null");

        var testAuthor = RandomTestUser();
        _db.Users.AddRange(new List<Author> { testAuthor });
        await _db.SaveChangesAsync();

        var result = await _repo.GetAuthorByEmail(testAuthor.Email);
        Assert.NotNull(result);
        Assert.Equal(testAuthor.Email, result.Email);
    }
}
