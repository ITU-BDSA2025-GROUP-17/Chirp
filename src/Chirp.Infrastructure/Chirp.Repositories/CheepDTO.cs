namespace Chirp.Repositories;

public class CheepDTO
{
    public required AuthorDTO Author;
    public required string Text;
    public required DateTime TimeStamp;
    public long? CheepId;
}