
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

        modelBuilder.Entity<AuditRecord>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.AuditData).IsRequired();
            entity.Property(a => a.Timestamp).IsRequired();
        });

        modelBuilder.Entity<AuditRecord>()
            .HasOne(ad => ad.AuditData)
            .WithOne(ar => ar.AuditRecord)
            .HasForeignKey<AuditRecord>(ar => ar.AuditDataId);
    }
}