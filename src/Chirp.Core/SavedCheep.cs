namespace Chirp.Core;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents a cheep saved/bookmarked by an author.
/// Stores the relationship between an author and a cheep, including when it was saved
/// </summary>
public class SavedCheep
{
    /// <summary>
    /// The ID of the author who saved the cheep
    /// </summary>
    public int AuthorId { get; set; }
    /// <summary>
    /// The author who saved the cheep
    /// </summary>
    public Author? Saver { set; get; }
    /// <summary>
    /// The ID of the cheep that was saved
    /// </summary>
    public long CheepId { set; get; }
    /// <summary>
    /// The cheep that was saved.
    /// </summary>
    public Cheep? Cheep { set; get; }
    /// <summary>
    /// The time when the cheep was saved.
    /// </summary>
    [Column("time_stamp")]  
    public DateTime TimeStamp { set; get; }
}