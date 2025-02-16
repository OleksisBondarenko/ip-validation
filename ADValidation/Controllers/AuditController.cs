using ADValidation.Decorators;
using ADValidation.DTOs.Audit;
using ADValidation.Enums;
using ADValidation.Mappers.Audit;
using ADValidation.Models;
using ADValidation.Models.Filter;
using ADValidation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ADValidation.Controllers;

[ApiController]
[Route("[controller]")]
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
    public async Task<ActionResult<IEnumerable<AuditRecordDTO>>> GetAuditRecords(
        [FromQuery] string? orderBy = "Timestamp", // Default sorting field
        [FromQuery] string? orderByDir = "ASC", // Default sorting direction
        [FromQuery] int limit = 10, // Default page size
        [FromQuery] int start = 0, // Default start index
        [FromQuery] int? entityList = null, // Optional entity list filter
        [FromQuery] string? search = null) // Global search term
    {
        try
        {
            // Convert query parameters to a filter request
            var request = new FilterRequest()
            {
                OrderBy = orderBy,
                OrderByDir = orderByDir,
                Limit = limit,
                Start = start,
                Search = search
            };

            // Apply entity list filter if provided
            if (entityList.HasValue)
            {
                request.Filters.Add(new Filter
                {
                    Type = "number",
                    Alias = "EntityList",
                    Value = new FilterValue { Input = entityList.Value.ToString() }
                });
            }

            var auditRecords = await _auditService.GetAllFilteredAsync(
                request.Filters,
                request.OrderBy,
                request.OrderByDir,
                request.Limit,
                request.Start,
                request.Search
            );

            var auditRecordDTOs = auditRecords.Select(_auditMapper.MapToAuditDataDTO);
            return Ok(auditRecordDTOs);
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

            return Ok(auditRecordDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit records.");
            return StatusCode(500, "Internal server error");
        }
    }
}