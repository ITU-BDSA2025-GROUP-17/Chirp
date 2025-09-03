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

        var chirps = new List<Cheep>();
        
        using (var reader = new StreamReader(path))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var savedChirps = csvReader.GetRecords<Cheep>();

            foreach (Cheep cheep in savedChirps)
            {
                chirps.Add(cheep);
            }
            chirps.Add(new(username, message, unixTime));
        }
        
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(chirps);
            writer.Flush();
        }
    }
    
}
public record Cheep(string Author, string Message, long Timestamp);
