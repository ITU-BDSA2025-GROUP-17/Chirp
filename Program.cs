using System.Globalization;
using Chirp.CLI;
using CsvHelper;
using DocoptNet;
using simpleDB;

class Program
{
    public static CSVDatabase<Cheep> db = new CSVDatabase<Cheep>("chirp_cli_db.csv");
    static void Main(string[] args)
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

        if (arguments["read"].IsTrue)
        {
            if (arguments["<limit>"].IsInt)
            {
                read(arguments["<limit>"].AsInt);
            }
            else
            {
                read();
            }
        } else if (arguments["cheep"].IsTrue)
        {
            if (arguments["<message>"].IsString)
            {
                cheep(arguments["<message>"].ToString());
            }
        }
    }

    static void read(int? limit = null)
    {
        UserInterface.PrintCheeps(db.Read(limit));
    }

    static void cheep(string message)
    {   
        string username = Environment.UserName;
        long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        db.Store(new Cheep(username, message, unixTime));
    }
    
}
public record Cheep(string Author, string Message, long Timestamp);
