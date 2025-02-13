
using ADValidation.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

public class ApplicationDbContext : DbContext
{
    public DbSet<AuditRecord> AuditRecords { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure your AuditRecord entity if needed
        modelBuilder.Entity<AuditRecord>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Data).IsRequired();
            entity.Property(a => a.Timestamp).IsRequired();
        });
    }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder  optionsBuilder)
    // {
    //     optionsBuilder.UseSqlite("Data Source=AuditDB.db");
    // }
}