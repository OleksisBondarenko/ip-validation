
using ADValidation.Models.Access;
using ADValidation.Models.Audit;
using ADValidation.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace ADValidation.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
{
    // public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AuditRecord> AuditRecords { get; set; }
    public DbSet<AccessPolicy> AccessPolicies { get; set; }

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
                
            // entity.Property(ar => ar.Name).HasMaxLength(200);
            entity.Property(ar => ar.Timestamp).IsRequired();
        });
        
        modelBuilder.Entity<AuditRecord>()
            .HasMany(a => a.AccessPolicies)
            .WithMany(p => p.AuditRecords)
            .UsingEntity(j => j.ToTable("AuditRecordAccessPolicies"));
        
    }
}