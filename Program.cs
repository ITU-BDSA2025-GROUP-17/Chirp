using System.Globalization;
using Chirp.CLI;
using SimpleDB;


public class Program
{
    private static CSVDatabase<Cheep> cheep_list = new CSVDatabase<Cheep>();
    private UserInterface userInterface = new UserInterface();
    
    
    public static void Main(string[] args)
    {
        
        
        const string usage = "@Welcome to chirp\n" +
                             "\n Usage:" +
                             "\nchirp read" +
                             "\nchirp cheep <message> " +
                             "\nchirp (-h | --help) " +
                             "\nchirp --version";
        
        
        var arguments = new DocoptNet.Docopt().Apply(usage, args, version: "1.0", exit: true);
        
        if (arguments[""].IsTrue) return;
        
        if (arguments["cheep"].IsTrue)
        {
            string message = arguments["<message>"].ToString();
            Cheep userShit = new Cheep(Environment.UserName, args[1], DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            cheep_list.Store(userShit);
        } else if (arguments["read"].IsTrue)
        {
            UserInterface.PrintCheeps(cheep_list.Read().ToList());
        }
        
        

        
    }
}



