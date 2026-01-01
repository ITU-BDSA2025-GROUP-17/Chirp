namespace Chirp.Core;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
/// <summary>
/// Represents a follow relationship between two authors.
/// One author (Follower) follows another author (Following).
/// </summary>
public class Follow
{
    /// <summary>
    /// The ID of the author who follows another author.
    /// </summary>
    public int FollowerId { set; get; }
    /// <summary>
    /// The author author who follows another author.
    /// </summary>
    public Author? Follower { set; get; }
    /// <summary>
    /// The ID of the author who is followed by another author.
    /// </summary>
    public int FollowingId { set; get; }
    /// <summary>
    /// The author author who who is followed by another author.
    /// </summary>
    public Author? Following { set; get; }
}