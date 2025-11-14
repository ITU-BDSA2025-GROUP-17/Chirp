namespace Chirp.Core;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class Follow
{
    public int FollowerId { set; get; }
    public Author? Follower { set; get; }
    public int FollowingId { set; get; }
    public Author? Following { set; get; }
}