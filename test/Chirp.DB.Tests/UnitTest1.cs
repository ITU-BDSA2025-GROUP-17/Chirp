namespace Chirp.DB.Tests;

using Chirp.Razor.wwwroot;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class CheepRepositoryTests
{
    private readonly ICheepRepository _repo;
    private readonly CheepDBContext _db;

    public CheepRepositoryTests()
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

    [Fact]
    public async Task CanAddAndRetrieveCheep()
    {
        // Arrange
        var a1 = new Author() { AuthorId = 1, Name = "Roger Histand", Email = "Roger+Histand@hotmail.com", Messages = new List<Cheep>() };

        var cheep = new Cheep{Author = a1,
            CheepId = 1, 
            AuthorId = 1, 
            Text = "Hello, world!", 
            TimeStamp = DateTime.Parse("2025-08-10 14:55:00")};
        
        a1.Messages.Add(cheep);
        
        _db.Users.AddRange(new List<Author>(){a1});
        _db.Messages.AddRange(new List<Cheep>(){cheep});
        _db.SaveChanges();
        
        // Act
        var cheeps = await _repo.ReadCheeps(null,0,1);

        // Assert
        Assert.Equal(cheeps[0].Author.Name, a1.Name);
        Assert.Equal(cheeps[0].Text, cheep.Text);
    }
}
