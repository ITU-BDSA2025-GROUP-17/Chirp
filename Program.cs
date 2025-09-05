using System.Globalization;
using CsvHelper;
using SimpleDB;

class Program
{
    public static CSVDatabase<Cheep> db = new SimpleDB.CSVDatabase<Cheep>();
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No arguments given. Exiting...");
            return; // stop Main here
        }

        if (args[0] == "read")
        {
            read(int.Parse(args[1]));
            return; // skip the rest of Main
        }
        
        if (args[0] == "cheep")
        {
            cheep(args[1]);
        }
    }

    static void read(int limit)
    {
        var chirps = db.Read(limit);
        
        foreach (var cheep in chirps)
        {
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).DateTime;

            Console.WriteLine(cheep.Author + " @ " + dateTime + ": " + cheep.Message);
        }
    }

    static void cheep(string message)
    {   
        string username = Environment.UserName;
        long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        db.Store(new Cheep(username, message, unixTime));
    }
    
}
public record Cheep(string Author, string Message, long Timestamp);
