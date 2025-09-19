namespace Chirp.CLI;

using DocoptNet;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class Program
{
    public static HttpClient client = new HttpClient();
    public static async Task Main(string[] args)
    {
        const string usage = @"Chirp CLI version.

        Usage:
          chirp read
          chirp read <limit>
          chirp cheep <message>
          chirp (-h | --help)
          chirp --version

        Options:
          -h --help     Show this screen.
          --version     Show version.
        ";

        var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;

        client.BaseAddress = new Uri("http://localhost:5241");
        if (arguments["read"].IsTrue)
        {
            if (arguments["<limit>"].IsInt)
            {
                await Read(arguments["<limit>"].AsInt);
            }
            else
            {
                await Read();
            }
        }
        else if (arguments["cheep"].IsTrue)
        {
            if (arguments["<message>"].IsString)
            {
                await Cheep(arguments["<message>"].ToString());
            }
        }
    }

    public static async Task Read(int? limit = null)
    {
        
        var cheeps = limit == null ?
            await client.GetFromJsonAsync<List<Cheep>>("cheeps") :
            await client.GetFromJsonAsync<List<Cheep>>("cheeps/"+limit);
        if (cheeps == null)
        {
            Console.WriteLine("Failed to fetch cheeps");
            return;
        }
        UserInterface.PrintCheeps(cheeps);
    }

    static async Task Cheep(string message)
    {   
        string username = Environment.UserName;
        long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await client.PostAsJsonAsync("cheep", new Cheep(username, message, unixTime));
    }
    
}
