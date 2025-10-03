using Microsoft.EntityFrameworkCore;


public class CheepDBContext : DbContext
{
    public DbSet<Cheep> Messages { get; set; }    
    
    public DbSet<Author> Users { get; set; }

    // base = superklasse i Java
    public CheepDBContext(DbContextOptions<CheepDBContext> options) : base(options)
    {
        
    }
}