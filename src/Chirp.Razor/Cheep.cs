

using System.ComponentModel.DataAnnotations.Schema;

public class Cheep
{
    [Column("message_id")]   
    public long CheepId { set; get; }
    public Author Author { set; get; }
    public string Text { set; get; }
    [Column("pub_date")]  
    public long PubDate { set; get; }

    [ForeignKey("authorId")]
    [Column("author_id")]
    public long AuthorId { set; get; }
}