using System.Text.Json;
using ADValidation.Data;
using ADValidation.DTOs.Audit;
using ADValidation.Enums;
using ADValidation.Mappers.Audit;
using ADValidation.Models.Audit;
using ADValidation.Models.Filter;
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
    }

    public async Task<IEnumerable<AuditRecord>> GetAllAsync(int page = 0, int pageSize = int.MaxValue)
    {
        List<AuditRecord> allRecords = await _context.AuditRecords.Include(ar => ar.AuditData).ToListAsync();
        return allRecords;
    }
    
     public async Task<IEnumerable<AuditRecord>> GetAllFilteredAsync(
        List<Filter> filters,
        string orderBy,
        string orderByDir,
        int limit,
        int start,
        string search)
    {
         var query = _context.AuditRecords
            .Include(ar => ar.AuditData)
            .AsNoTracking()
            .AsQueryable();
        
        // Apply global search
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(ar =>
                EF.Functions.Like(ar.AuditData.IpAddress, $"%{search}%") ||
                EF.Functions.Like(ar.AuditData.Domain, $"%{search}%") ||
                EF.Functions.Like(ar.AuditData.Hostname, $"%{search}%"));
        }

        // Apply filters
        if (filters != null && filters.Any())
        {
            foreach (var filter in filters)
            {
                query = ApplyFilter(query, filter);
            }
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(orderBy))
        {
            query = ApplySorting(query, orderBy, orderByDir);
        }

        // Apply pagination
        query = query.Skip(start).Take(limit);

        return await query.ToListAsync();
    }

    public async Task<int> GetTotalFilteredAsync(
        List<Filter> filters,
        string orderBy,
        string orderByDir,
        string search)
    {
        var query = _context.AuditRecords
            .Include(ar => ar.AuditData)
            .AsNoTracking()
            .AsQueryable();

        // Apply global search
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(ar =>
                EF.Functions.Like(ar.AuditData.IpAddress, $"%{search}%") ||
                EF.Functions.Like(ar.AuditData.Domain, $"%{search}%") ||
                EF.Functions.Like(ar.AuditData.Hostname, $"%{search}%"));
        }

        // Apply filters
        if (filters != null && filters.Any())
        {
            foreach (var filter in filters)
            {
                query = ApplyFilter(query, filter);
            }
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(orderBy))
        {
            query = ApplySorting(query, orderBy, orderByDir);
        }

        return await query.CountAsync();
    }

    private IQueryable<AuditRecord> ApplyFilter(IQueryable<AuditRecord> query, Filter filter)
    {
        switch (filter.Type.ToLower())
        {
            case "date":
                query = ApplyDateFilter(query, filter);
                break;
            case "string":
                query = ApplyStringFilter(query, filter);
                break;
            case "stringarray":
                query = ApplyStringArrayFilter(query, filter);
                break;
            // case "int":
            //     query = ApplyStringFilter(query, filter);
            //     break;
            // Add more filter types as needed
            default:
                throw new ArgumentException($"Unsupported filter type: {filter.Type}");
        }

        return query;
    }
    
    // private IQueryable<AuditRecord> ApplyIntFilter(IQueryable<AuditRecord> query, Filter filter)
    // {
    //     return filter.Alias.ToLower() switch
    //     {
    //         "actiontype" => ApplyIpAddressFilter(query, filter),
    //         _ => throw new ArgumentException($"Unsupported string field: {filter.Alias}")
    //     };
    // }
    
    private IQueryable<AuditRecord> ApplyStringFilter(IQueryable<AuditRecord> query, Filter filter)
    {
        return filter.Alias.ToLower() switch
        {
            "ipaddress" => ApplyIpAddressFilter(query, filter),
            "hostname" => ApplyHostnameFilter(query, filter), 
            "resourcename" => ApplyResourceFilter(query, filter), 
            "audittype" => ApplyAuditTypeFilter(query, filter),
            _ => throw new ArgumentException($"Unsupported string field: {filter.Alias}")
        };
    }
    
    private IQueryable<AuditRecord> ApplyStringArrayFilter(IQueryable<AuditRecord> query, Filter filter)
    {
        return filter.Alias.ToLower() switch
        {
            "audittype" => ApplyAuditTypeFilter(query, filter),
            "domain" => ApplyDomainFilter(query, filter), 
            _ => throw new ArgumentException($"Unsupported string field: {filter.Alias}")
        };
    }
    
    private IQueryable<AuditRecord> ApplyAuditTypeFilter(IQueryable<AuditRecord> query, Filter filter)
    {
        // Ensure the filter value is not null or empty
        if (string.IsNullOrEmpty(filter.Value.Input))
        {
            throw new ArgumentException("Filter value for audit_type cannot be null or empty.");
        }

        // // Get the filter value as a string
        // bool isValid = int.TryParse(filter.Value.Input, out int value);

        // if (!isValid)
        // {
        //     throw new ArgumentException($"Unsupported int field: {filter.Alias}");
        // }
        
        List<int> inputs;
        try
        {
            inputs =
                JsonSerializer.Deserialize<List<string>>(filter.Value.Input)
                    .Select(input => int.Parse(input))
                    .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error while parsing {nameof(inputs)}");
        }

        // Apply the filter based on the alias
        return filter.Alias.ToLower() switch
        {
            "audittype" => query.Where(ar =>
                ar.AuditData != null && // Ensure AuditData is not null
                inputs.Contains((int)ar.AuditType)),
            _ => throw new ArgumentException($"Unsupported string field: {filter.Alias}")
        };
    }
    
    private IQueryable<AuditRecord> ApplyIpAddressFilter(IQueryable<AuditRecord> query, Filter filter)
    {
        // Ensure the filter value is not null or empty
        if (string.IsNullOrEmpty(filter.Value.Input))
        {
            throw new ArgumentException("Filter value for IP address cannot be null or empty.");
        }

        // Get the filter value as a string
        var value = filter.Value.Input;

        // Apply the filter based on the alias
        return filter.Alias.ToLower() switch
        {
            "ipaddress" => query.Where(ar =>
                ar.AuditData != null && // Ensure AuditData is not null
                !string.IsNullOrEmpty(ar.AuditData.IpAddress) && // Ensure IpAddress is not null
                ar.AuditData.IpAddress.Contains(value)), // Case-insensitive search
            _ => throw new ArgumentException($"Unsupported string field: {filter.Alias}")
        };
    }
    
    private IQueryable<AuditRecord> ApplyHostnameFilter(IQueryable<AuditRecord> query, Filter filter)
    {
        // Ensure the filter value is not null or empty
        if (string.IsNullOrEmpty(filter.Value.Input))
        {
            throw new ArgumentException("Filter value for IP address cannot be null or empty.");
        }

        // Get the filter value as a string
        var value = filter.Value.Input;

        // Apply the filter based on the alias
        return filter.Alias.ToLower() switch
        {
            "hostname" => query.Where(ar =>
                ar.AuditData != null && // Ensure AuditData is not null
                !string.IsNullOrEmpty(ar.AuditData.Hostname) && // Ensure IpAddress is not null
                ar.AuditData.Hostname.Contains(value, StringComparison.OrdinalIgnoreCase)), // Case-insensitive search
            _ => throw new ArgumentException($"Unsupported string field: {filter.Alias}")
        };
    }
    
    private IQueryable<AuditRecord> ApplyDomainFilter(IQueryable<AuditRecord> query, Filter filter)
    {
        // Ensure the filter value is not null or empty
        if (string.IsNullOrEmpty(filter.Value.Input))
        {
            throw new ArgumentException("Filter value for IP address cannot be null or empty.");
        }

        List<string> inputs = JsonSerializer.Deserialize<List<string>>(filter.Value.Input) 
                              ?? throw new ArgumentNullException(nameof(filter.Value.Input), "Input cannot be null");

        // Create a HashSet for O(1) lookups (case-insensitive)
        var inputSet = new HashSet<string>(inputs, StringComparer.OrdinalIgnoreCase);

        return filter.Alias.ToLowerInvariant() switch
        {
            "domain" => query.Where(ar =>
                ar.AuditData != null && 
                (inputSet.Contains(string.Empty) ||
                inputSet.Contains(ar.AuditData.Domain, StringComparer.OrdinalIgnoreCase))),
            _ => throw new ArgumentException($"Unsupported string field: {filter.Alias}")
        };
    }
    
    private IQueryable<AuditRecord> ApplyResourceFilter(IQueryable<AuditRecord> query, Filter filter)
    {
        // Ensure the filter value is not null or empty
        if (string.IsNullOrEmpty(filter.Value.Input))
        {
            throw new ArgumentException("Filter value for resource cannot be null or empty.");
        }

        // Get the filter value as a string
        var value = filter.Value.Input;

        // Apply the filter based on the alias
        return filter.Alias.ToLower() switch
        {
            "resourcename" => query.Where(ar =>
                ar.AuditData != null && // Ensure AuditData is not null
                !string.IsNullOrEmpty(ar.ResourceName) && // Ensure IpAddress is not null
                ar.ResourceName.Contains(value, StringComparison.OrdinalIgnoreCase)), // Case-insensitive search
            _ => throw new ArgumentException($"Unsupported string field: {filter.Alias}")
        };
    }

    private IQueryable<AuditRecord> ApplyDateFilter(IQueryable<AuditRecord> query, Filter filter)
    {
        return filter.Alias.ToLower() switch
        {
            "timestamp" => ApplyDateRangeFilter(query, filter.Value),
            _ => throw new ArgumentException($"Unsupported date field: {filter.Alias}")
        };
    }

    private IQueryable<AuditRecord> ApplyDateRangeFilter(IQueryable<AuditRecord> query, FilterValue value)
    {
        if (value.From.HasValue && value.To.HasValue)
        {
            return query.Where(ar => ar.Timestamp >= value.From && ar.Timestamp <= value.To);
        }
        else if (value.From.HasValue)
        {
            return query.Where(ar => ar.Timestamp >= value.From);
        }
        else if (value.To.HasValue)
        {
            return query.Where(ar => ar.Timestamp <= value.To);
        }

        return query;
    }

    private IQueryable<AuditRecord> ApplySorting(IQueryable<AuditRecord> query, string orderBy, string orderByDir)
    {
        switch (orderBy.ToLower())
        {
            case "timestamp":
                return orderByDir.ToUpper() switch
                {
                    "ASC" => query.OrderBy(ar => ar.Timestamp),
                    "DESC" => query.OrderByDescending(ar => ar.Timestamp),
                    _ => throw new ArgumentException($"Unsupported sorting direction: {orderByDir}")
                };
            case "audittype":
                return orderByDir.ToUpper() switch
                {
                    "ASC" => query.OrderBy(ar => ar.AuditType),
                    "DESC" => query.OrderByDescending(ar => ar.AuditType),
                    _ => throw new ArgumentException($"Unsupported sorting direction: {orderByDir}")
                };

            case "resourcename":
                return orderByDir.ToUpper() switch
                {
                    "ASC" => query.OrderBy(ar => ar.ResourceName),
                    "DESC" => query.OrderByDescending(ar => ar.ResourceName),
                    _ => throw new ArgumentException($"Unsupported sorting direction: {orderByDir}")
                };
            case "domain":
                return orderByDir.ToUpper() switch
                {
                    "ASC" => query.OrderBy(ar => ar.AuditData.Domain),
                    "DESC" => query.OrderByDescending(ar => ar.AuditData.Domain),
                    _ => throw new ArgumentException($"Unsupported sorting direction: {orderByDir}")
                };
            case "ipaddress":
                return orderByDir.ToUpper() switch
                {
                    "ASC" => query.OrderBy(ar => ar.AuditData.IpAddress),
                    "DESC" => query.OrderByDescending(ar => ar.AuditData.IpAddress),
                    _ => throw new ArgumentException($"Unsupported sorting direction: {orderByDir}")
                };
            case "hostname":
                return orderByDir.ToUpper() switch
                {
                    "ASC" => query.OrderBy(ar => ar.AuditData.Hostname),
                    "DESC" => query.OrderByDescending(ar => ar.AuditData.Hostname),
                    _ => throw new ArgumentException($"Unsupported sorting direction: {orderByDir}")
                };
            // case "name":
            //     return orderByDir.ToUpper() switch
            //     {
            //         "ASC" => query.OrderBy(ar => ar.Name),
            //         "DESC" => query.OrderByDescending(ar => ar.Name),
            //         _ => throw new ArgumentException($"Unsupported sorting direction: {orderByDir}")
            //     };
            // Add more cases for other sortable fields
            default:
                throw new ArgumentException($"Unsupported orderBy field: {orderBy}");
        }
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
