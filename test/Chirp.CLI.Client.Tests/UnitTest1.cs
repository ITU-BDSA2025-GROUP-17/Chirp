namespace Chirp.CLI.Client.Tests;

using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class UnitTest1
{
    [Fact]
    public async Task TestReadFirstCheep() 
    {
        HttpClient client = new HttpClient();
        
        client.BaseAddress = new Uri("http://localhost:5241");
        var cheeps = await client.GetFromJsonAsync<List<Cheep>>("cheeps/");
        
        Assert.Equal("Hello, BDSA students!",cheeps[0].Message);
        Assert.Equal("ropf",cheeps[0].Author);
        Assert.Equal(1690891760,cheeps[0].Timestamp);
    }
}
