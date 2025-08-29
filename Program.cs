if (args.Length == 0) return;

if (args[0] == "read") {
    List<String> cheeps = new(File.ReadAllLines("chirp_cli_db.csv"));

    foreach (String cheep in cheeps) {
        String[]  split = cheep.Split('\"');
        if (split.Length < 2) continue;
        String user = split[0].Replace(",", "");
        String message = split[1];
        DateTimeOffset time = DateTimeOffset.FromUnixTimeSeconds(int.Parse(split[2].Replace(",", "")));
        String date = "" + time;
        Console.WriteLine(user+" @ "+date.Remove(date.Length-7)+": "+message);
    }
}else if (args[0] == "cheep")
{
    if (args.Length < 2) return;
    String message = args[1];
    File.AppendAllText("chirp_cli_db.csv","\n"+Environment.UserName + ",\"" + message + "\"," + DateTimeOffset.UtcNow.ToUnixTimeSeconds());
}