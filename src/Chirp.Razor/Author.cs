namespace Chirp.Razor.wwwroot;

using System.ComponentModel.DataAnnotations.Schema;

public class Author
{
    public long AuthorId { set; get; }
    public string Name { set; get; }
    public string Email { set; get; }
    public ICollection<Cheep> Messages { set; get; }
    
    
}