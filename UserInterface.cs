namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (Cheep currentCheep in cheeps) {
            DateTimeOffset time = DateTimeOffset.FromUnixTimeSeconds(currentCheep.Timestamp).LocalDateTime;
            Console.WriteLine(currentCheep.Author+" @ "+time.DateTime+": "+currentCheep.Message);
        }
    }
}