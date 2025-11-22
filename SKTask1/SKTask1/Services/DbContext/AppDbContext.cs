using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

public class AppDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>()
            .HasKey(d => d.Id);
    }
}