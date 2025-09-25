using Microsoft.Data.Sqlite;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{

    public List<CheepViewModel> GetCheeps()
    {
        var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH")
                     ?? Path.Combine(Path.GetTempPath(), "chirp.db");

        using var connection = new SqliteConnection($"Data Source={dbPath}");
        //Console.WriteLine($"Using database at: {dbPath}");
        
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT user.username, message.text, message.pub_date FROM message INNER JOIN user ON message.author_id = user.user_id";
        using var reader = command.ExecuteReader();
        var cheeps = new List<CheepViewModel>();
        while (reader.Read())
        {
            cheeps.Add(new CheepViewModel(reader.GetString(0), reader.GetString(1), UnixTimeStampToDateTimeString(reader.GetDouble(2))));
        }
        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH")
                     ?? Path.Combine(Path.GetTempPath(), "chirp.db");
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT user.username, message.text, message.pub_date FROM message INNER JOIN user ON message.author_id = user.user_id WHERE user.username = @author";
        using var reader = command.ExecuteReader();
        var cheeps = new List<CheepViewModel>();
        while (reader.Read())
        {
            cheeps.Add(new CheepViewModel(reader.GetString(0), reader.GetString(1), UnixTimeStampToDateTimeString(reader.GetDouble(2))));
        }
        return cheeps;
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
