namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> chirps)
    {
        Console.Write(FormatCheeps(chirps));
    }

    public static string FormatCheeps(IEnumerable<Cheep> chirps)
    {
        string result = "";
        foreach (var cheep in chirps)
        {
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).DateTime.ToLocalTime();
            result += cheep.Author + " @ " + dateTime + ": " + cheep.Message + "\n";
        }
        return result;
    }
}