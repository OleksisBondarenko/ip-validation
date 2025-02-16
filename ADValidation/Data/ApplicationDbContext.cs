
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


        modelBuilder.Entity<AuditData>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.HasOne(a => a.AuditRecord)
                .WithOne(ar => ar.AuditData)
                .HasForeignKey<AuditData>(a => a.AuditRecordId)
                .OnDelete(DeleteBehavior.Cascade); 

            entity.Property(a => a.IpAddress).HasMaxLength(50);
            entity.Property(a => a.Hostname).HasMaxLength(100);
            entity.Property(a => a.UserName).HasMaxLength(100);
            entity.Property(a => a.Domain).HasMaxLength(100);
        });

        modelBuilder.Entity<AuditRecord>(entity =>
        {
            entity.HasKey(ar => ar.Id);

            entity.HasOne(ar => ar.AuditData)
                .WithOne(a => a.AuditRecord)
                .HasForeignKey<AuditData>(a => a.AuditRecordId);

            entity.Property(ar => ar.Name).HasMaxLength(200);
            entity.Property(ar => ar.Timestamp).IsRequired();
        });
        
    }
}