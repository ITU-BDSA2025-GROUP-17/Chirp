using Xunit.Abstractions;
using Xunit.Sdk;

namespace Chirp.DB.Tests;

using Chirp.Razor.wwwroot;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class CheepRepositoryTests
{
    private ICheepRepository _repo;
    private CheepDBContext _db;

    public void SetUpCheepRepositoryTests()
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
    }

    public string RandomString(int length)
    {
        char[] generatedString = new char[length];
        for (int i = 0; i<length ; i++)
        {
            generatedString[i] = (char) (new Random().Next(33, 127));
        }
        return new string(generatedString);
    }

    [Fact]
    public async Task ReadCheepTest()
    {
        SetUpCheepRepositoryTests();
        // Arrange
        var username = RandomString(new Random().Next(1, 20));
        var a1 = new Author() { AuthorId = 1, Name = username, Email = username+"@hotmail.com", Messages = new List<Cheep>() };

        var message = RandomString(150);
        var cheep = new Cheep{Author = a1,
            CheepId = 1, 
            AuthorId = 1, 
            Text = message, 
            TimeStamp = DateTime.Parse("2025-08-10 14:55:00")};
        
        a1.Messages.Add(cheep);
        
        _db.Users.AddRange(new List<Author>(){a1});
        _db.Messages.AddRange(new List<Cheep>(){cheep});
        _db.SaveChanges();
        
        // Act
        var cheeps = await _repo.ReadCheeps(null,0,1);

        // Assert
        Assert.Equal(cheeps[0].Author.Name, username);
        Assert.Equal(cheeps[0].Text, message);
    }

    [Fact]
    public async Task WriteCheepTest()
    {
        SetUpCheepRepositoryTests();
        
        
    }

    [Fact]
    public async Task CreateAuthorTest()
    {
        SetUpCheepRepositoryTests();
        for (int i = 0; i < 1000; i++)
        {
            string authorName = RandomString(new Random().Next(1,20));
            string authorEmail = authorName + "@MPGA.gov";

            try
            {
                await _repo.CreateAuthor(authorName, authorEmail);
            }
            catch (DbUpdateException e)
            {
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
        
    }

    [Fact]
    public async Task ReadAuthorFromEmailTest()
    {
        SetUpCheepRepositoryTests();
        
    }
    
}
