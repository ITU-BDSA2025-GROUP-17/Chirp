namespace Chirp.Repositories;

using System.Net;
using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Entity Framework Core database context for the Chirp application.
/// Responsible for configuring and exposing database sets and relationships.
/// </summary>
public class CheepDBContext : IdentityDbContext<Author, IdentityRole<int>, int>
{
    /// <summary>
    /// Database table for cheeps posted by authors.
    /// </summary>
    public DbSet<Cheep> Cheeps { get; set; }

    /// <summary>
    /// Database table representing follow relationships between authors.
    /// </summary>
    public DbSet<Follow> Follows { get; set; }

    /// <summary>
    /// Database table representing cheeps saved by authors.
    /// </summary>
    public DbSet<SavedCheep> SavedCheeps { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheepDBContext"/>.
    /// </summary>
    /// <param name="options">Configuration options for the database context.</param>
    public CheepDBContext(DbContextOptions<CheepDBContext> options) : base(options)
    {

    }

    /// <summary>
    /// Configures entity relationships and composite keys using the Fluent API.
    /// </summary>
    /// <param name="builder">The model builder used to configure entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure the Follow entity (many-to-many self-referencing relationship)
        builder.Entity<Follow>(entity =>
        {
            // Define composite primary key
            entity.HasKey(f => new { f.FollowerId, f.FollowingId }); //primary keys

            // Configure the follower relationship
            entity.HasOne(f => f.Follower)
                .WithMany()
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the following relationship
            entity.HasOne(f => f.Following)
                .WithMany()
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure the SavedCheep entity (many-to-many relationship with payload)
        builder.Entity<SavedCheep>(entity =>
        {
            // Define composite primary key
            entity.HasKey(sc => new { sc.AuthorId, sc.CheepId });

            // Configure relationship to the author who saved the cheep
            entity.HasOne(sc => sc.Saver)
                .WithMany(a => a.SavedCheeps)
                .HasForeignKey(sc => sc.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship to the saved cheep
            entity.HasOne(sc => sc.Cheep)
                .WithMany()
                .HasForeignKey(sc => sc.CheepId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

}