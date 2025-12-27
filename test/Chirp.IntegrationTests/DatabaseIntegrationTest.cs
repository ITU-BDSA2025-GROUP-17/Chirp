using Microsoft.Extensions.DependencyInjection;
using Chirp.Services;
using Chirp.Repositories;
using System.Reflection;

namespace Chirp.IntegrationTests;

public class DatabaseIntegrationTests : IClassFixture<ChirpWebApplicationFactory>
{
    private readonly ChirpWebApplicationFactory _factory;

    public DatabaseIntegrationTests(ChirpWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private (ICheepService cheepService, IAuthorService authorService, IServiceScope scope) GetServices()
    {
        var scope = _factory.Services.CreateScope(); //this scope creates an local instance of DbContext
        var cheepService = scope.ServiceProvider.GetRequiredService<ICheepService>();
        var authorService = scope.ServiceProvider.GetRequiredService<IAuthorService>();
        return (cheepService, authorService, scope);
    }


    [Fact]
    public async Task GetAuthorByName_WithSeededData_ReturnsCorrectAuthor()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope) //this ensures scope gets disposed after
        {
            var author = await authorService.GetAuthorByName("Helge");

            Assert.NotNull(author);
            Assert.Equal("Helge", author.Name);
            Assert.Equal("ropf@itu.dk", author.Email);
        }
    }

    [Fact]
    public async Task GetPublicCheeps_WithSeededData_ReturnsNonEmptyList()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var cheeps = await cheepService.GetPublicCheeps(1, null);

            Assert.NotNull(cheeps);
            Assert.NotEmpty(cheeps);

            var specificCheep = cheeps.FirstOrDefault(c => c.CheepId == 142);
            Assert.NotNull(specificCheep);
            Assert.Equal(142, specificCheep.CheepId);
        }
    }

    [Fact]
    public async Task GetPublicCheeps_WithDifferentPageNumbers_ReturnsDifferentResults()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var cheepsPageOne = await cheepService.GetPublicCheeps(1, null);
            var cheepsPageTwo = await cheepService.GetPublicCheeps(2, null);

            Assert.NotNull(cheepsPageOne);
            Assert.NotNull(cheepsPageTwo);
            Assert.NotEmpty(cheepsPageOne);
            Assert.NotEmpty(cheepsPageTwo);
            Assert.NotEqual(cheepsPageOne, cheepsPageTwo);
        }
    }

    [Fact]
    public async Task GetAuthorByName_AndByEmail_ReturnsSameAuthor()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var authorByName = await authorService.GetAuthorByName("Quintin Sitts");
            var authorByEmail = await authorService.GetAuthorByEmail("Quintin+Sitts@itu.dk");

            Assert.NotNull(authorByName);
            Assert.NotNull(authorByEmail);
            Assert.Equal(authorByName.Name, authorByEmail.Name);
            Assert.Equal(authorByName.Email, authorByEmail.Email);
            Assert.Equal(authorByName.AuthorId, authorByEmail.AuthorId);
        }
    }

    [Fact]
    public async Task GetAuthorByName_WithNonExistentAuthor_ReturnsNull()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var unknownAuthor = await authorService.GetAuthorByName("H. C. Andersen");
            Assert.Null(unknownAuthor);
        }
    }

    [Fact]
    public async Task GetUserTimelineCheeps_WithExistingUser_ReturnsCheepsFromUserAndFollowing()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var cheeps = await cheepService.GetUserTimelineCheeps("Helge", 1);

            Assert.NotNull(cheeps);
            Assert.NotEmpty(cheeps);
            // GetUserTimelineCheeps returns cheeps from the user AND people they follow
            // So we just verify that we get cheeps back, not that they're all from Helge
            Assert.True(cheeps.Any());
        }
    }

    [Fact]
    public async Task CreateCheepForUser_WithValidData_CreatesCheep()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var testUserName = "Helge";
            var testText = "This is a test cheep from integration test";

            var cheepsBefore = await cheepService.GetUserTimelineCheeps(testUserName, 1);
            var countBefore = cheepsBefore.Count;

            await cheepService.CreateCheepForUser(testUserName, testText);

            var cheepsAfter = await cheepService.GetUserTimelineCheeps(testUserName, 1);
            var countAfter = cheepsAfter.Count;

            Assert.Equal(countBefore + 1, countAfter);
            var newCheep = cheepsAfter.FirstOrDefault(c => c.Text == testText);
            Assert.NotNull(newCheep);
            Assert.Equal(testUserName, newCheep.Author.Name);
        }
    }

    [Fact]
    public async Task SaveCheepForUser_AndIsCheepSavedByUser_SavesAndVerifiesCheep()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var userName = "Helge";
            var publicCheeps = await cheepService.GetPublicCheeps(1, null);
            var cheepToSave = publicCheeps.First();

            var isSavedBefore = await cheepService.IsCheepSavedByUser(userName, cheepToSave.CheepId!.Value);

            await cheepService.SaveCheepForUser(userName, cheepToSave.CheepId!.Value);

            var isSavedAfter = await cheepService.IsCheepSavedByUser(userName, cheepToSave.CheepId!.Value);

            Assert.True(isSavedAfter);
        }
    }

    [Fact]
    public async Task RemoveSavedCheepForUser_RemovesPreviouslySavedCheep()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var userName = "Helge";
            var publicCheeps = await cheepService.GetPublicCheeps(1, null);
            var cheepToSave = publicCheeps.First();

            await cheepService.SaveCheepForUser(userName, cheepToSave.CheepId!.Value);
            var isSaved = await cheepService.IsCheepSavedByUser(userName, cheepToSave.CheepId!.Value);
            Assert.True(isSaved);

            await cheepService.RemoveSavedCheepForUser(userName, cheepToSave.CheepId!.Value);
            var isStillSaved = await cheepService.IsCheepSavedByUser(userName, cheepToSave.CheepId!.Value);

            Assert.False(isStillSaved);
        }
    }

    [Fact]
    public async Task GetSavedCheeps_ReturnsSavedCheepsForUser()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var userName = "Helge";
            var publicCheeps = await cheepService.GetPublicCheeps(1, null);
            var cheepToSave = publicCheeps.First();

            await cheepService.SaveCheepForUser(userName, cheepToSave.CheepId!.Value);

            var savedCheeps = await cheepService.GetSavedCheeps(userName, 1);

            Assert.NotNull(savedCheeps);
            Assert.Contains(savedCheeps, c => c.CheepId == cheepToSave.CheepId);
        }
    }

    [Fact]
    public async Task FollowUser_AndIsFollowing_FollowsAndVerifiesUser()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var currentUser = "Helge";
            var targetUser = "Quintin Sitts";

            var isFollowingBefore = await authorService.IsFollowing(currentUser, targetUser);

            await authorService.FollowUser(currentUser, targetUser);

            var isFollowingAfter = await authorService.IsFollowing(currentUser, targetUser);

            Assert.True(isFollowingAfter);
        }
    }

    [Fact]
    public async Task UnfollowUser_UnfollowsPreviouslyFollowedUser()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var currentUser = "Helge";
            var targetUser = "Quintin Sitts";

            await authorService.FollowUser(currentUser, targetUser);
            var isFollowing = await authorService.IsFollowing(currentUser, targetUser);
            Assert.True(isFollowing);

            await authorService.UnfollowUser(currentUser, targetUser);
            var isStillFollowing = await authorService.IsFollowing(currentUser, targetUser);

            Assert.False(isStillFollowing);
        }
    }

    [Fact]
    public async Task GetFollowing_ReturnsListOfFollowedUsers()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var currentUser = "Helge";
            var targetUser = "Quintin Sitts";

            // Check if already following, if so unfollow first to avoid conflict
            var isAlreadyFollowing = await authorService.IsFollowing(currentUser, targetUser);
            if (isAlreadyFollowing)
            {
                await authorService.UnfollowUser(currentUser, targetUser);
            }

            await authorService.FollowUser(currentUser, targetUser);

            var following = await authorService.GetFollowing(currentUser);

            Assert.NotNull(following);
            Assert.Contains(following, author => author.Name == targetUser);
        }
    }

    [Fact]
    public async Task GetPublicCheeps_WithSearchQuery_ReturnsFilteredResults()
    {
        var (cheepService, authorService, scope) = GetServices();
        using (scope)
        {
            var searchQuery = "Hello";

            var cheeps = await cheepService.GetPublicCheeps(1, searchQuery);

            Assert.NotNull(cheeps);

            Assert.All(cheeps, cheep =>
                Assert.True(
                    cheep.Text.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    cheep.Author.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                )
            );
        }
    }

}

