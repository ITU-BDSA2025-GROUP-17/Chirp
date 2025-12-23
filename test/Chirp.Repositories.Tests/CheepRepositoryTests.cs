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
        return new Cheep
        {
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
        if (_db == null) throw new NullReferenceException("_db is null");

        for (var i = 0; i < 1000; i++)
        {
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
        if (_db == null) throw new NullReferenceException("_db is null");

        var testAuthor = Utility.RandomTestUser(true);
        var testAuthorDto = new AuthorDTO
        {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };
        _db.Users.AddRange(new List<Author> { testAuthor });
        await _db.SaveChangesAsync();
        for (var i = 0; i < 1000; i++)
        {
            var testCheep = RandomTestCheep(testAuthor, 150, i);
            var testCheepDto = new CheepDTO
            {
                Text = testCheep.Text,
                Author = testAuthorDto,
                TimeStamp = testCheep.TimeStamp
            };

            await _cheepRepo.CreateCheep(testCheepDto);

            var success = false;
            foreach (var cheep in _db.Cheeps)
            {
                if (cheep.Text == testCheep.Text)
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
        if (_db == null) throw new NullReferenceException("_db is null");

        //make user
        var testAuthor = Utility.RandomTestUser(true);
        var testAuthorDto = new AuthorDTO
        {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };
        // make follow user
        var testFollow = Utility.RandomTestUser(true);
        var testFollowAuthDto = new AuthorDTO
        {
            AuthorId = testFollow.Id,
            Name = testFollow.UserName!,
            Email = testFollow.Email!
        };

        _db.Users.AddRange(new List<Author> { testAuthor, testFollow });
        await _db.SaveChangesAsync();

        var followCheeps = await _cheepRepo.ReadCheepsFromFollowers(new List<string> { testAuthorDto.Name }, 0, 100);

        Assert.True(followCheeps.Count == 0);

        // var _authRepo = new AuthorRepository(_db);

        var cheep = RandomTestCheep(testFollow, 150, 1);
        await _db.Cheeps.AddAsync(cheep);


        await _db.SaveChangesAsync();
        // await _authRepo.Follow(testAuthorDto, testFollowAuthDto);
        if (testAuthor.Following == null)
        {
            testAuthor.Following = new List<Follow>();
        }
        testAuthor.Following.Add(
             new Follow()
             {
                 Follower = testAuthor,
                 FollowingId = testFollow.Id
             }
         );
        await _db.SaveChangesAsync();

        followCheeps = await _cheepRepo.ReadCheepsFromFollowers(new List<string> { testAuthorDto.Name, testFollowAuthDto.Name }, 0, 100);

        bool check = false;
        foreach (var followCheep in followCheeps)
        {
            if (followCheep.CheepId == cheep.CheepId) { check = true; break; }
        }
        Assert.True(check);
    }

    [Fact]
    public async Task saveCheepsTest()
    {
        SetUpCheepRepositoryTests();
        if (_cheepRepo == null) throw new NullReferenceException("_cheepRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        //make user
        var testAuthor = Utility.RandomTestUser(true);
        var testAuthorDto = new AuthorDTO
        {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };
        // make follow user
        var testFollow = Utility.RandomTestUser(true);
        var testFollowAuthDto = new AuthorDTO
        {
            AuthorId = testFollow.Id,
            Name = testFollow.UserName!,
            Email = testFollow.Email!
        };

        _db.Users.AddRange(new List<Author> { testAuthor, testFollow });
        await _db.SaveChangesAsync();

        var cheep = RandomTestCheep(testFollow, 150, 1);

        await _db.Cheeps.AddAsync(cheep);
        await _db.SaveChangesAsync();

        cheep = _db.Cheeps.ToList()[0];
        var cheepDto = new CheepDTO
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
        Assert.False(await _cheepRepo.IsSaved(testAuthorDto, cheepDto));
        await _cheepRepo.SaveCheep(testAuthorDto, cheepDto);
        Assert.True(await _cheepRepo.IsSaved(testAuthorDto, cheepDto));
    }

    [Fact]
    public async Task DeleteSavedCheepsTest()
    {
        SetUpCheepRepositoryTests();
        if (_cheepRepo == null) throw new NullReferenceException("_cheepRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        //make user
        var testAuthor = Utility.RandomTestUser(true);
        var testAuthorDto = new AuthorDTO
        {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };
        // make follow user
        var testFollow = Utility.RandomTestUser(true);
        var testFollowAuthDto = new AuthorDTO
        {
            AuthorId = testFollow.Id,
            Name = testFollow.UserName!,
            Email = testFollow.Email!
        };

        _db.Users.AddRange(new List<Author> { testAuthor, testFollow });
        await _db.SaveChangesAsync();

        var cheep = RandomTestCheep(testFollow, 150, 1);

        await _db.Cheeps.AddAsync(cheep);
        await _db.SaveChangesAsync();

        cheep = _db.Cheeps.ToList()[0];
        var cheepDto = new CheepDTO
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
        Assert.False(await _cheepRepo.IsSaved(testAuthorDto, cheepDto));
        await _cheepRepo.SaveCheep(testAuthorDto, cheepDto);
        Assert.True(await _cheepRepo.IsSaved(testAuthorDto, cheepDto));
        await _cheepRepo.RemoveSavedCheep(testAuthorDto, cheepDto);
        Assert.False(await _cheepRepo.IsSaved(testAuthorDto, cheepDto));
    }

    [Fact]
    public async Task ReadSavedCheepsTest()
    {
        SetUpCheepRepositoryTests();
        if (_cheepRepo == null) throw new NullReferenceException("_cheepRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        //make user
        var testAuthor = Utility.RandomTestUser(true);
        var testAuthorDto = new AuthorDTO
        {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };
        // make follow user
        var testFollow = Utility.RandomTestUser(true);
        var testFollowAuthDto = new AuthorDTO
        {
            AuthorId = testFollow.Id,
            Name = testFollow.UserName!,
            Email = testFollow.Email!
        };

        _db.Users.AddRange(new List<Author> { testAuthor, testFollow });
        await _db.SaveChangesAsync();

        var cheep = RandomTestCheep(testFollow, 150, 1);

        await _db.Cheeps.AddAsync(cheep);
        await _db.SaveChangesAsync();

        cheep = _db.Cheeps.ToList()[0];
        var cheepDto = new CheepDTO
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
        Assert.False(await _cheepRepo.IsSaved(testAuthorDto, cheepDto));
        await _cheepRepo.SaveCheep(testAuthorDto, cheepDto);
        Assert.True(await _cheepRepo.IsSaved(testAuthorDto, cheepDto));
        var readList = await _cheepRepo.ReadSavedCheeps(testAuthor.Id, 0, 100);

        bool check = false;
        foreach (var savedCheep in readList)
        {
            if (savedCheep.CheepId == cheep.CheepId) { check = true; break; }
        }
        Assert.True(check);
    }

    [Fact]
    public async Task GetCheepByIdTest()
    {
        SetUpCheepRepositoryTests();
        if (_cheepRepo == null) throw new NullReferenceException("_cheepRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        //make user
        var testAuthor = Utility.RandomTestUser(true);
        var testAuthorDto = new AuthorDTO
        {
            AuthorId = testAuthor.Id,
            Name = testAuthor.UserName!,
            Email = testAuthor.Email!
        };

        _db.Users.AddRange(new List<Author> { testAuthor });
        await _db.SaveChangesAsync();

        var cheep = RandomTestCheep(testAuthor, 150, 1);

        await _db.Cheeps.AddAsync(cheep);
        await _db.SaveChangesAsync();

        cheep = _db.Cheeps.ToList()[0];
        var cheepDto = new CheepDTO
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

        var cheepeee = await _cheepRepo.GetCheepById(cheep.CheepId);
        Assert.True(cheep.CheepId == cheepeee!.CheepId);

    }

    [Fact]
    public async Task WriteCheepExceedingLimitTest()
    {
        SetUpCheepRepositoryTests();
        if (_cheepRepo == null) throw new NullReferenceException("_cheepRepo is null");
        if (_db == null) throw new NullReferenceException("_db is null");

        //make user
        var testAuthor = Utility.RandomTestUser(true);
        _db.Users.AddRange(new List<Author> { testAuthor });
        await _db.SaveChangesAsync();

        var cheep = RandomTestCheep(testAuthor, 200, 1);

        var cheepDto = new CheepDTO
        {
            Text = cheep.Text,
            TimeStamp = cheep.TimeStamp,
            Author = new()
            {
                AuthorId = testAuthor.Id,
                Name = testAuthor.UserName!,
                Email = testAuthor.Email!
            }
        };

        await _cheepRepo.CreateCheep(cheepDto);
        Assert.Empty(_db.Cheeps.ToList());


    }
}
