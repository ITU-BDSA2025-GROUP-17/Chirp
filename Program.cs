using System.Globalization;
using Chirp.CLI;
using SimpleDB;


public class Program
{
    private static CSVDatabase<Cheep> cheep_list = new CSVDatabase<Cheep>();
    private UserInterface userInterface = new UserInterface();
    
    
    public static void Main(string[] args)
    {
        
        const string usage = 
            @"Welcome to chirp

Usage:
  chirp read
  chirp cheep <message>
  chirp (-h | --help)
  chirp --version";

        // Parse args med Docopt
        var arguments = new DocoptNet.Docopt().Apply(usage, args, version: "1.0", exit: true);

        // POST en cheep
        if (arguments.TryGetValue("cheep", out var cheepVal) && cheepVal.IsTrue)
        {
            var message = arguments.TryGetValue("<message>", out var msgVal) ? msgVal.ToString() : "";
            var cheep = new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            cheep_list.Store(cheep);
        }
        // READ cheeps
        else if (arguments.TryGetValue("read", out var readVal) && readVal.IsTrue)
        {
            UserInterface.PrintCheeps(cheep_list.Read().ToList());
        }
        else
        {
            // Ingen match = vis usage
            Console.WriteLine(usage);
        }
        
        
        
        
        
        /*
        
        const string usage = "@Welcome to chirp\n" +
                             "\n Usage:" +
                             "\nchirp read" +
                             "\nchirp cheep <message> " +
                             "\nchirp (-h | --help) " +
                             "\nchirp --version";
        
        
        var arguments = new DocoptNet.Docopt().Apply(usage, args, version: "1.0", exit: true);
        
        
        if (arguments["cheep"].IsTrue)
        {
            string message = arguments["<message>"].ToString();
            Cheep userShit = new Cheep(Environment.UserName, args[1], DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            cheep_list.Store(userShit);
        } else if (arguments["read"].IsTrue)
        {
            UserInterface.PrintCheeps(cheep_list.Read().ToList());
        }
        
        

        */
    }
}



