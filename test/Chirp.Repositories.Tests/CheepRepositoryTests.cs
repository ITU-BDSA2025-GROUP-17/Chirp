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

public class CheepRepositoryTests
{
    private ICheepRepository? _cheepRepo;
    private CheepDBContext? _db;

    private void SetUpCheepRepositoryTests()
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

        _cheepRepo = provider.GetRequiredService<ICheepRepository>();
    }

    private Cheep RandomTestCheep(Author author, int length, double? time)
    {
        var actualTime = time == null ? new Random().NextDouble() : (double)time;
        var message = Utility.RandomString(length);
        return new Cheep {
            Author = author,
            Text = message, 
            TimeStamp = DateTime.UnixEpoch.AddSeconds(actualTime),
        };
    }

    [Fact]
    public async Task ReadCheepTest()
    {
        SetUpCheepRepositoryTests();
        if (_cheepRepo == null) throw new NullReferenceException("_cheepRepo is null");
        if(_db == null) throw new NullReferenceException("_db is null");
        
        for(var i = 0; i < 1000; i++) {
            // Arrange
            var testAuthor = Utility.RandomTestUser(true);
			var cheep = RandomTestCheep(testAuthor, 150, i);
            testAuthor.Cheeps!.Add(cheep);

            _db.Users.AddRange(new List<Author> { testAuthor });
            _db.Cheeps.AddRange(new List<Cheep> { cheep });
            await _db.SaveChangesAsync();
            

            // Act
            var cheeps = await _cheepRepo.ReadCheeps(null, 0, 1);

            // Assert
            Assert.Equal(cheeps[0].Author.Name, testAuthor.UserName);
            Assert.Equal(cheeps[0].Text, cheep.Text);
        }
    }

    [Fact]
    public async Task WriteCheepTest()
    {
        SetUpCheepRepositoryTests();
        if (_cheepRepo == null) throw new NullReferenceException("_cheepRepo is null");
        if(_db == null) throw new NullReferenceException("_db is null");
        
        var testAuthor = Utility.RandomTestUser(true);
        var testAuthorDto = new AuthorDTO {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
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

            await _cheepRepo.CreateCheep(testCheepDto);

            var success = false;
            foreach (var cheep in _db.Cheeps)
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

    [Fact]
    public async Task ReadCheepsFromFollowersTest()
    {
        SetUpCheepRepositoryTests();
        if (_cheepRepo == null) throw new NullReferenceException("_cheepRepo is null");
        if(_db == null) throw new NullReferenceException("_db is null");
        
        var testAuthor = Utility.RandomTestUser(true);
        var testAuthorDto = new AuthorDTO {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };
        var testFollow = Utility.RandomTestUser(true);
        var testFollowAuthDto = new AuthorDTO {
            AuthorId = testFollow.Id,
            Name = testFollow.UserName!,
            Email = testFollow.Email!
        };
        _db.Users.AddRange(new List<Author> { testAuthor, testFollow });
        await _db.SaveChangesAsync();

        var list = await _cheepRepo.ReadCheepsFromFollowers(testAuthorDto.Name, 0, 100);
        
        Assert.True(list.Count == 0);
        
        var _authRepo = new AuthorRepository(_db);
       
        var cheep = RandomTestCheep(testAuthor, 150, 1);
        await _db.Cheeps.AddAsync(cheep);
        
        
        await _db.SaveChangesAsync();
        await _authRepo.Follow(testAuthorDto, testFollowAuthDto);
       
        list = await _cheepRepo.ReadCheepsFromFollowers(testAuthorDto.Name, 0, 100);
        var followCheeps = await _cheepRepo.ReadCheeps(testFollowAuthDto.Name, 0,100 );
        Assert.Contains(followCheeps[0], list);
    }
    
    

    /*[Fact]
    public async Task WriteCheepExceedingLimitTest()
    {
        SetUpCheepRepositoryTests();
        if (_cheepRepo == null) throw new NullReferenceException("_cheepRepo is null");
        if(_db == null) throw new NullReferenceException("_db is null");
        
        Author testAuthor = RandomTestUser();
        Cheep cheepExceedingLimit = RandomTestCheep(testAuthor, 200, null);

        testAuthor.Messages.Add(cheepExceedingLimit);

        _db.Users.AddRange(new List<Author> { testAuthor });
        _db.Messages.AddRange(new List<Cheep> { cheepExceedingLimit });
        await _db.SaveChangesAsync();
    }*/
}
