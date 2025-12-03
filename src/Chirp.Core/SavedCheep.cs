namespace Chirp.Core;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class SavedCheep
{
    public int AuthorId { get; set; }
    public Author? Saver { set; get; }
    public long CheepId { set; get; }
    public Cheep? Cheep { set; get; }
    [Column("time_stamp")]  
    public DateTime TimeStamp { set; get; }
}