namespace Chirp.Repositories;
using Core;

public class AuthorDTO
{
    public string Name;
    public string Email;
    public ICollection<CheepDTO> Messages;
}