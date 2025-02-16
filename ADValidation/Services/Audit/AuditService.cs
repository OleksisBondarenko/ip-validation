using ADValidation.Models.Audit;
using Microsoft.EntityFrameworkCore;

namespace ADValidation.Services;

public class AuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(AuditRecord record)
    {
        _context.AuditRecords.Add(record);
        await _context.SaveChangesAsync();
    }

    public async Task<AuditRecord> GetByIdAsync(Guid id)
    {
        return await _context.AuditRecords.FindAsync(id);
    }`

    public async Task<IEnumerable<AuditRecord>> GetAllAsync(int page, int pageSize)
    {
        return await _context.AuditRecords.ToListAsync();
    }

    public async Task UpdateAsync(AuditRecord record)
    {
        _context.AuditRecords.Update(record);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var record = await _context.AuditRecords.FindAsync(id);
        if (record != null)
        {
            _context.AuditRecords.Remove(record);
            await _context.SaveChangesAsync();
        }
    }
}
