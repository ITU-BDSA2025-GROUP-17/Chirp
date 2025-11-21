using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Chirp.Web;
using NuGet.Protocol;

namespace ChirpEndToEndTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    private Process _serverProcess;

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
        await Page.GotoAsync("http://localhost:7273/");
        var cheeps = await Page.Locator("#messagelist li").AllTextContentsAsync();

        /* For test-testing
        foreach (var cheep in cheeps)
        {
            Console.WriteLine(cheep);
        }*/

        Assert.That(cheeps.Count, Is.GreaterThan(0));

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

    [OneTimeTearDown]
    public async Task Cleanup()
    {
        _serverProcess.Kill(entireProcessTree: true);
        _serverProcess.Dispose();
    }
}
