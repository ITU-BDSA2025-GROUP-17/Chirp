namespace Chirp.Core;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Cheep
{
    [Column("message_id")]   
    public long CheepId { set; get; }
    
    [StringLength(160)]
    public required string Text { set; get; }
    [Column("time_stamp")]  
    public DateTime TimeStamp { set; get; }

    [ForeignKey("AuthorId")]
    [Column("author_id")]
    public long AuthorId { set; get; }
    
    public Author? Author { set; get; }
}