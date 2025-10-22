namespace Chirp.Repositories;


using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


public class CheepDBContext : IdentityDbContext<Author, IdentityRole<int>, int>
{
    public DbSet<Cheep> Messages { get; set; }
    
    //public DbSet<Author> Authors { get; set; }

    // base = superklasse i Java
    public CheepDBContext(DbContextOptions<CheepDBContext> options) : base(options)
    {
        
    }
    /*protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Author>(entity => {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
        });
    }*/
    
    
}