namespace Chirp.Razor.wwwroot;

public class CheepDTO
{
    public CheepDTO(Author author, string text, DateTime timeStamp, long cheepId)
    {
        Author = author;
        Text = text;
        TimeStamp = timeStamp;
        CheepId = cheepId;
    }
    public Author Author;
    public string Text;
    public DateTime TimeStamp;
    public long CheepId;
}