namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> chirps)
    {
        foreach (var cheep in chirps)
        {
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).DateTime.ToLocalTime();

            Console.WriteLine(cheep.Author + " @ " + dateTime + ": " + cheep.Message);
        }
    }
}