namespace Chirp.Razor.wwwroot;

using System.ComponentModel.DataAnnotations.Schema;

public class Cheep
{
    [Column("message_id")]   
    public long CheepId { set; get; }
    public string Text { set; get; }
    [Column("time_stamp")]  
    public DateTime TimeStamp { set; get; }

    [ForeignKey("authorId")]
    [Column("author_id")]
    public long AuthorId { set; get; }
    
    public Author Author { set; get; }
}