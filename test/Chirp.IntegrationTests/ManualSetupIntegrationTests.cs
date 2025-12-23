using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Reflection;

namespace Chirp.IntegrationTests;

// this is the manuel way of doing integration tests
// - we apply UseTestServer() explicit (page 898 DOTNET ASP book)

// because we use .NET 6+ minimal hosting model in Program.cs, then its easier to use WebApplicationFactory

public class ManualSetupIntegrationTests
{
    [Fact]
    public async Task SimpleEndpoint_ReturnsSuccess_ManualSetup()
    {
        // STEP 1: build a Host with WebHostBuilder
        // this is the manual way - building everything 
        var hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                // STEP 2: *** KEY *** - User TestServer instead of Kestrel (picture on page 897)
                // TestServer = In-memory HTTP server (no network, no ports)
                // Kestrel = true HTTP server (listens on a port)
                webHost.UseTestServer();

                // STEP 3: Config our mini-app manuel
                webHost.ConfigureServices(services =>
                {
                    // add needed services
                    services.AddRouting();
                });

                webHost.Configure(app =>
                {
                    // add simple endpoint
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/test", async context =>
                        {
                            await context.Response.WriteAsync("Hello from TestServer!");
                        });
                    });
                });
            });

        // STEP 4: Start host (builds and starts in-memory app)
        using var host = await hostBuilder.StartAsync();

        // STEP 5: *** IMPORTANT *** - GetTestClient() gives HttpClient config to TestServer
        // this client makes requests directly to TestServer (no network!)
        var client = host.GetTestClient();

        // STEP 6: make HTTP request - goes through all middleware pipeline in-memory
        var response = await client.GetAsync("/test");
        var content = await response.Content.ReadAsStringAsync();

        // STEP 7: Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("Hello from TestServer!", content);

        // STEP 8: Stop host (cleanup)
        await host.StopAsync();
    }

    [Fact]
    public async Task TestServer_HandlesMultipleRequests()
    {
        // Setup: Build test host manuel
        var hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();  // <----- MAGIC: In-memory server!

                webHost.ConfigureServices(services =>
                {
                    services.AddRouting();
                });

                webHost.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/hello/{name}", async context =>
                        {
                            var name = context.Request.RouteValues["name"];
                            await context.Response.WriteAsync($"Hello, {name}!");
                        });
                    });
                });
            });

        using var host = await hostBuilder.StartAsync();
        var client = host.GetTestClient();

        // Act: make more requests manuel
        var response1 = await client.GetAsync("/hello/Alice");
        var response2 = await client.GetAsync("/hello/Bob");

        var content1 = await response1.Content.ReadAsStringAsync();
        var content2 = await response2.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal("Hello, Alice!", content1);
        Assert.Equal("Hello, Bob!", content2);

        await host.StopAsync();
    }
}


