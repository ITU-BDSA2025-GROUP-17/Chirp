if (args.Length == 0) return;

if (args[0] == "read") {
    Read();
}else if (args[0] == "cheep")
{
    if (args.Length < 2) return;
    Cheep(args[1]);
}

void Read() 
{
    List<String> cheeps = new(File.ReadAllLines("chirp_cli_db.csv"));

    foreach (String currentCheep in cheeps) {
        String[]  split = currentCheep.Split('\"');
        if (split.Length < 2) continue;
        String user = split[0].Replace(",", "");
        String message = split[1];
        DateTimeOffset time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(split[2].Replace(",", ""))).LocalDateTime;
        String date = "" + time.DateTime;
        Console.WriteLine(user+" @ "+date+": "+message);
    }
}

void Cheep(String message)
{
    String name = Environment.UserName;
    long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();  
    File.AppendAllText("chirp_cli_db.csv","\n"+ name + ",\"" + message + "\"," + time);
}
