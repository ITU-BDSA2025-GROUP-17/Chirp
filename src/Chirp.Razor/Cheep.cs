

using System.ComponentModel.DataAnnotations.Schema;

public class Cheep
{
    [Column("message_id")]   
    public long messageId { set; get; }
    public Author author { set; get; }
    public string text { set; get; }
    [Column("pub_date")]  
    public long pubDate { set; get; }

    [ForeignKey("authorId")]
    [Column("author_id")]
    public long authorId { set; get; }
}