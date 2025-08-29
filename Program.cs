List<String> cheeps = new(File.ReadAllLines("../../../chirp_cli_db.csv"));
cheeps.RemoveAt(0);

foreach (String cheep in cheeps)
{
    String[]  split = cheep.Split('\"');
    String user = split[0].Replace(",", "");
    String message = split[1];
    String date = split[2].Replace(",", "");
    Console.WriteLine(user+" @ "+date+": "+message);
}