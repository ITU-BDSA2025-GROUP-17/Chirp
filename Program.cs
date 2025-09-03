using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using CsvHelper;

class Program
{
    static void Main(string[] args)
    {
        String filePath = "C:\\Users\\2000b\\Documents\\Chirp\\bin\\chirp_cli_db.csv";
        if (args.Length == 0)
        {
            Console.WriteLine("No arguments given. Exiting...");
            return; // stop Main here
        }

        if (args[0] == "read")
        {
            read(filePath);
            return; // skip the rest of Main
        }
        
        if (args[0] == "cheep")
        {
            cheep(filePath, args[1]);
        }

    }

    static void read(string path)
    {
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var chirps = csv.GetRecords<Cheep>();
            
            foreach (Cheep cheep in chirps)
            {
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).DateTime;

                Console.WriteLine(cheep.Author + " @ " + dateTime + ": " + cheep.Message);
            }

        }
    }

    static void cheep(string path, string message)
    {   
        string username = Environment.UserName;
        long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string timestampString = unixTime.ToString();
        string newLine = username+','+'"'+message+'"'+','+timestampString;
        
        File.AppendAllText(path, newLine + Environment.NewLine);
    }
    
}


public class Cheep
{
    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }
}