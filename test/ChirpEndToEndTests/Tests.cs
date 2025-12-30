using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NuGet.Protocol;

namespace ChirpEndToEndTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    private Process _serverProcess;
    private string _url = "http://localhost:7273/";
    public override BrowserNewContextOptions ContextOptions() => new() { IgnoreHTTPSErrors = true };

    [OneTimeSetUp]
    public async Task Init()
    {
        string projectPath = "../../../../../src/Chirp.Web/Chirp.Web.csproj";
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --launch-profile testing",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        _serverProcess = Process.Start(startInfo)!;

        _serverProcess.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
        _serverProcess.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
        

        // Wait for server to start
        string? line;
        while ((line = await _serverProcess.StandardOutput.ReadLineAsync()) != null)
        {
            Console.WriteLine(line);

            if (line.Contains("Now listening on", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Server ready");
                break;
            }
        }

    }

    [Test]
    public async Task ReadCheep()
    {
        await Page.GotoAsync(_url);
        var cheeps = await Page.Locator("#messagelist li").AllTextContentsAsync();

        /* For test-testing
        foreach (var cheep in cheeps)
        {
            Console.WriteLine(cheep);
        }*/

        Assert.That(cheeps.Count, Is.EqualTo(32));

        // ***First Cheep on page 1***
        // Author
        Assert.That(cheeps[0], Contains.Substring("Jacqualine Gilcoine"));


        // Text
        Assert.That(cheeps[0], Contains.Substring("Starbuck now is what we hear the worst."));
        
        // ***Last Cheep on page 1***
        // i = 31 because there should be 32 cheeps per page
        // Author
        Assert.That(cheeps[31], Contains.Substring("Jacqualine Gilcoine"));


        // Text
        Assert.That(cheeps[31], Contains.Substring("With back to my friend, patience!"));
    }

    [Test]
    public async Task SearchCheep()
    {   
        await Page.GotoAsync(_url);

        await Page.FillAsync("#Search", "Starbuck");
        await Page.ClickAsync("input[type=submit]");

        Assert.That(Page.Url, Does.Contain("search=Starbuck"));
        var cheeps = await Page.Locator("#messagelist li").AllTextContentsAsync();
        foreach(string cheep in cheeps)
        {
            Assert.That(cheep, Contains.Substring("Starbuck"));
        }
    }

    [Test]
    public async Task PageChange()
    {
        await Page.GotoAsync(_url);

        await Page.ClickAsync("text=Next");
        
        var cheeps = await Page.Locator("#messagelist li").AllTextContentsAsync();
        Assert.That(cheeps[0], Contains.Substring("In the morning of the wind, some few splintered planks, of what present avail to him."));
        Assert.That(cheeps[31], Contains.Substring("He walked slowly back the lid."));

        await Page.ClickAsync("text=Prev");
        
        cheeps = await Page.Locator("#messagelist li").AllTextContentsAsync();
        Assert.That(cheeps[0], Contains.Substring("Starbuck now is what we hear the worst."));
        Assert.That(cheeps[31], Contains.Substring("With back to my friend, patience!"));
    }

    [Test]
    public async Task ViewUserTimeline()
    {   
        await Page.GotoAsync(_url);

        await Page.ClickAsync("text=Jacqualine Gilcoine");
        
        var cheeps = await Page.Locator("#messagelist li").AllTextContentsAsync();
        // since Jacqualine has more than 32 Cheeps index 0 and 31 can still be used to check
        Assert.That(cheeps[0], Contains.Substring("Starbuck now is what we hear the worst."));
        Assert.That(cheeps[31], Contains.Substring("Now, amid the cloud-scud."));

        // Next page of Jacqualine's Cheeps
        await Page.ClickAsync("text=Next");
        
        cheeps = await Page.Locator("#messagelist li").AllTextContentsAsync();
        Assert.That(cheeps[0], Contains.Substring("What a relief it was the place examined."));
        Assert.That(cheeps[31], Contains.Substring("At eleven there was movement in the teeth that he was in its niches."));
    }

    public record UserInfo(string Username, string Email);
    // Used to setup the test that require a user to be logged in
    public async Task<UserInfo> UserTestInit()
    {
        await Page.GotoAsync(_url);

        var username = "test_user_" + Guid.NewGuid().ToString();
        var email = username+"@tester.dk";
        
        // Registration
        await Page.ClickAsync("text=Register");

        await Page.FillAsync("#Input_Username", username);
        await Page.FillAsync("#Input_Email", email);
        await Page.FillAsync("#Input_Password", "Secure1!");
        await Page.FillAsync("#Input_ConfirmPassword", "Secure1!");
        await Page.ClickAsync("#registerSubmit");
        
        await Page.ClickAsync("#confirm-link");

        // Login
        await Page.ClickAsync("text=Login");
        
        await Page.FillAsync("#Input_Username", username);
        await Page.FillAsync("#Input_Password", "Secure1!");
        await Page.ClickAsync("#login-submit");
        
        await Page.ClickAsync("text=public timeline");

        return new UserInfo(username, email);
    }
    
    [Test] // We won't be testing OAuth registration through Playwright
    public async Task RegisterLoginLogoutUser()
    {   
        var user = await UserTestInit();
        
        await Page.ClickAsync("text=my timeline");

        Assert.That(Page.Url, Is.EqualTo(_url+user.Username));
        Assert.That(await Page.IsVisibleAsync("text=Logout"));

        await Page.ClickAsync("text=Logout");

        Assert.That(await Page.IsVisibleAsync("text=Register"));
    }

    [Test]
    public async Task FollowUnfollowUser()
    {
        await UserTestInit();

        // Follow
        await Page.ClickAsync("button[name='follow'][value='Jacqualine Gilcoine']");

        Assert.That(await Page.IsVisibleAsync("button[name='unfollow'][value='Jacqualine Gilcoine']"));

        // Check feed changed
        await Page.ClickAsync("text=my timeline");
        Assert.That(!await Page.IsVisibleAsync("text=There are no cheeps so far."));

        await Page.ClickAsync("text=public timeline");
        
        // Unfollow
        await Page.ClickAsync("button[name='unfollow'][value='Jacqualine Gilcoine']");
        Assert.That(await Page.IsVisibleAsync("button[name='follow'][value='Jacqualine Gilcoine']"));
        
        // Check feed empty again
        await Page.ClickAsync("text=my timeline");
        Assert.That(await Page.IsVisibleAsync("text=There are no cheeps so far."));
    }

    [NonParallelizable]
    [Test, Order(100)] // Make sure this test is run after the others
    public async Task PostCheep()
    {
        var user = await UserTestInit();

        await Page.FillAsync("#Text", "Test cheep");
        await Page.ClickAsync("input[type='submit'][value='Share']");

        var cheeps = await Page.Locator("#messagelist li").AllTextContentsAsync();

        Assert.That(cheeps.Count, Is.EqualTo(32));

        Assert.That(cheeps[0], Contains.Substring(user.Username));
        Assert.That(cheeps[0], Contains.Substring("Test cheep"));
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        _serverProcess.Kill(entireProcessTree: true);
        _serverProcess.Dispose();
    }
}
