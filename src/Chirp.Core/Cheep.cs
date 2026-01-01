namespace Chirp.Core;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents a single cheep (message) posted by an author.
/// A cheep contains text content and a timestamp.
/// </summary>
public class Cheep
{
    /// <summary>
    /// Unique identifier for the cheep.
    /// </summary>
    [Column("message_id")]   
    public long CheepId { set; get; }
    
    /// <summary>
    /// The textual content of the cheep.
    /// Limited to 160 characters.
    /// </summary>
    [StringLength(160)]
    public required string Text { set; get; }
    /// <summary>
    /// The time when the cheep was created.
    /// </summary>
    [Column("time_stamp")]  
    public DateTime TimeStamp { set; get; }
    
    /// <summary>
    /// The author who wrote the cheep.
    /// </summary>
    public Author? Author { set; get; }
}