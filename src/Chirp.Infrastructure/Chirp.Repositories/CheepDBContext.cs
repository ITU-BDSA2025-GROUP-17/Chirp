namespace Chirp.Repositories;

using System.Net;
using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


public class CheepDBContext : IdentityDbContext<Author, IdentityRole<int>, int>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<SavedCheep> SavedCheeps { get; set; }

    //public DbSet<Author> Authors { get; set; }

    // base = superklasse i Java
    public CheepDBContext(DbContextOptions<CheepDBContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Follow>(entity =>
        {
            entity.HasKey(f => new { f.FollowerId, f.FollowingId }); //primary keys

            entity.HasOne(f => f.Follower)
                .WithMany()
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.Following)
                .WithMany()
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<SavedCheep>(entity =>
        {
            entity.HasKey(sc => new { sc.AuthorId, sc.CheepId });

            entity.HasOne(sc => sc.Saver)
                .WithMany(a => a.SavedCheeps)
                .HasForeignKey(sc => sc.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(sc => sc.Cheep)
                .WithMany()
                .HasForeignKey(sc => sc.CheepId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }




}