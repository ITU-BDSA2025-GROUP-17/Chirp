using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace ChirpEndToEndTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    private Process _serverProcess;

    [SetUp]
    public async Task Init()
    {
        
    }

    [TearDown]
    public async Task Cleanup()
    {
        _serverProcess.Kill();
        _serverProcess.Dispose();
    }
}
