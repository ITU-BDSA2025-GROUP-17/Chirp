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
        IResponse? response = await Page.GotoAsync("http://localhost:7273/");
        var headers = await response!.HeadersArrayAsync();
        foreach (var h in headers)
        {
            Console.WriteLine(h.ToJson());
        }

        Console.WriteLine(await response!.TextAsync());
    }

    [OneTimeTearDown]
    public async Task Cleanup()
    {
        _serverProcess.Kill(entireProcessTree: true);
        _serverProcess.Dispose();
    }
}
