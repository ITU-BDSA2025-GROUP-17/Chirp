
using System.ComponentModel.DataAnnotations.Schema;

public class Author
{
    [Column("user_id")]
    public long authorId { set; get; }
    public string name { set; get; }
    public string email { set; get; }
    [NotMapped]
    public ICollection<Cheep> messages { set; get; }
    
    
}