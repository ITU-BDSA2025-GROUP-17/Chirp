using Microsoft.Data.Sqlite;

namespace Chirp.Razor.wwwroot;

public class CheepDBFacade : IDBFacade<CheepViewModel>
{
    private string dbPath;
    public CheepDBFacade(string dbPath)
    {
        this.dbPath = dbPath;   
    }
    public List<CheepViewModel> Read(int start, int limit)
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = 
            "SELECT user.username, message.text, message.pub_date " +
            "FROM message " +
            "INNER JOIN user ON message.author_id = user.user_id " +
            "ORDER BY message.pub_date " +
            "LIMIT @limit " +
            "OFFSET @start";
        command.Parameters.AddWithValue("@limit", limit);
        command.Parameters.AddWithValue("@start", start);
        
        var reader = command.ExecuteReader();
        var cheeps = new List<CheepViewModel>();
        while (reader.Read())
        {
            cheeps.Add(new CheepViewModel(reader.GetString(0), reader.GetString(1), UnixTimeStampToDateTimeString(reader.GetDouble(2))));
        }

        return cheeps;


    }

    public List<CheepViewModel> ReadByName(string username, int start, int limit)
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT user.username, message.text, message.pub_date " +
            "FROM message INNER JOIN user ON message.author_id = user.user_id " +
            "WHERE user.username = @username " +
            "ORDER BY message.pub_date " +
            "LIMIT @limit " +
            "OFFSET @start";
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@limit", limit);
        command.Parameters.AddWithValue("@start", start);
        
        var reader = command.ExecuteReader();
        var cheeps = new List<CheepViewModel>();
        while (reader.Read())
        {
            cheeps.Add(new CheepViewModel(reader.GetString(0), reader.GetString(1), UnixTimeStampToDateTimeString(reader.GetDouble(2))));
        }
        return cheeps;
        
    }

    public void Store(CheepViewModel item)
    {
        throw new NotImplementedException();
    }
    
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}