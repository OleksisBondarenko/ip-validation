using ADValidation.Decorators;
using ADValidation.DTOs.Audit;
using ADValidation.Enums;
using ADValidation.Mappers.Audit;
using ADValidation.Models;
using ADValidation.Models.Filter;
using ADValidation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ADValidation.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuditController : ControllerBase
{
    private readonly ILogger<AuditController> _logger;
    private readonly AuditService _auditService;
    private readonly AuditMapper _auditMapper;

    public AuditController(ILogger<AuditController> logger, AuditService auditService)
    {
        _logger = logger;
        _auditService = auditService;
        _auditMapper = new AuditMapper();
    }

    // GET: /Audit (Get all audit records with advanced filtering, sorting, and pagination)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResponseGetListAudit>>> GetAuditRecords(
        [FromQuery] string? orderBy = "Timestamp", // Default sorting field
        [FromQuery] string? orderByDir = "DESC", // Default sorting direction
        [FromQuery] int? limit = 10, // Default page size
        [FromQuery] int? start = 0, // Default start index
        [FromQuery] string? search = null, // Global search term 
        [FromQuery] string? auditType = null,
        [FromQuery] string? timeAfter = null,
        [FromQuery] string? timeBefore = null
        ) // Global search term 
    {
        try
        {
            // Convert query parameters to a filter request
            var request = new FilterRequest()
            {
                Filters = new List<Filter>(),
                OrderBy = orderBy,
                OrderByDir = orderByDir,
                Limit = limit.Value,
                Start = start.Value,
                Search = search
            };

            // Apply auditType filter if provided
            if (!string.IsNullOrEmpty(auditType))
            {
                request.Filters.Add(new Filter
                {
                    Type = "number",
                    Alias = "AuditType",
                    Value = new FilterValue { Input = auditType }
                });
            }
            
            // Apply timeAfter filter if provided
            if (!string.IsNullOrEmpty(timeAfter))
            {
                request.Filters.Add(new Filter
                {
                    Type = "date",
                    Alias = "TimeAfter",
                    Value = new FilterValue { Input = timeAfter }
                });
            }
            
            // Apply timeBefore filter if provided
            if (!string.IsNullOrEmpty(timeBefore))
            {
                request.Filters.Add(new Filter
                {
                    Type = "date",
                    Alias = "TimeBefore",
                    Value = new FilterValue { Input = timeBefore }
                });
            }

            int totalRecords = await _auditService.GetTotalFilteredAsync(
                request.Filters,
                request.OrderBy,
                request.OrderByDir,
                request.Search
                );
            
            var auditRecords = await _auditService.GetAllFilteredAsync(
                request.Filters,
                request.OrderBy,
                request.OrderByDir,
                request.Limit,
                request.Start,
                request.Search
            );

            var auditRecordDTOs = auditRecords.Select(_auditMapper.MapToAuditDataDTO);
            var resp = new ResponseGetListAudit
            {
                Data = auditRecordDTOs,
                TotalCount = totalRecords
            };
            
            return Ok(resp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit records.");
            return StatusCode(500, "Internal server error");
        }
    }
    
    
    [HttpPost("search")] // Use POST for complex filtering
    public async Task<ActionResult<IEnumerable<AuditRecordDTO>>> GetAuditRecords([FromBody] FilterRequest request)
    {
        try
        { 
            int totalRecords = await _auditService.GetTotalFilteredAsync(
                request.Filters,
                request.OrderBy,
                request.OrderByDir,
                request.Search
            );
            
            // Apply filtering, sorting, and pagination
            var auditRecords = await _auditService.GetAllFilteredAsync(
                request.Filters,
                request.OrderBy,
                request.OrderByDir,
                request.Limit,
                request.Start,
                request.Search
            );

            // Map to DTOs
            var auditRecordDTOs = auditRecords.Select(_auditMapper.MapToAuditDataDTO);

            var resp = new ResponseGetListAudit
            {
                Data = auditRecordDTOs,
                TotalCount = totalRecords
            };
            return Ok(resp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit records.");
            return StatusCode(500, "Internal server error");
        }
    }

    public class ResponseGetListAudit
    {
        public IEnumerable<AuditRecordDTO> Data { get; set; }
        public int TotalCount { get; set; }
    } 
}