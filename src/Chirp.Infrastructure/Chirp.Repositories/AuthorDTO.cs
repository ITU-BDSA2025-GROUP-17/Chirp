namespace Chirp.Repositories;

public class AuthorDTO
{
    public required string Name;
    public required string Email;
    public long? AuthorId;
    public ICollection<CheepDTO>? Messages;
}