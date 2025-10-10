namespace Chirp.Razor.wwwroot;

public class AuthorDTO
{
    public string Name;
    public string Email;
    public ICollection<Cheep> Messages;
}