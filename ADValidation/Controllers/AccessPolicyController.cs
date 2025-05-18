
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADValidation.Data;
using ADValidation.DTOs.AccessPolicy;
using ADValidation.Mappers.AccessRule;
using ADValidation.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace ADValidation.Controllers;


[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class AccessPolicyController : ControllerBase
{
    private readonly AccessPolicyMapper _mapper;
    private readonly ApplicationDbContext _context;

    public AccessPolicyController(ApplicationDbContext context)
    {
        _context = context;
        _mapper = new AccessPolicyMapper();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccessPolicyDto>>> GetAll()
    {
        var policies = await _context.AccessPolicies.ToListAsync();
        var dtos = policies.Select(p => _mapper.MapToDto(p)).ToList();
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccessPolicyDto>> GetById(long id)
    {
        var policy = await _context.AccessPolicies.FindAsync(id);
        if (policy == null) return NotFound();

        return Ok(_mapper.MapToDto(policy));
    }

    [HttpPost]
    public async Task<ActionResult<AccessPolicyDto>> Create([FromBody]AccessPolicyDto createDto)
    {
        var entity = _mapper.MapFromCreateDto(createDto);
        
        await HandleProperOrder(entity);
        _context.AccessPolicies.Add(entity);
        
        await _context.SaveChangesAsync();

        var dto = _mapper.MapToDto(entity);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] AccessPolicyDto updateDto)
    {
        var entity = await _context.AccessPolicies.FindAsync(id);
        if (entity == null) return NotFound();

        entity.Name = updateDto.Name;
        entity.Description = updateDto.Description;
        entity.IsActive = updateDto.IsActive;
        entity.IpFilterRules = updateDto.IpFilterRules;
        entity.Action = updateDto.Action;
        // entity.Resource = updateDto.Resource;
        // entity.ValidationTypes = updateDto.Validators;
        
        await HandleProperOrder(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpPatch("reorder")]
    public async Task<IActionResult> ReorderPolicy([FromBody] ReorderPolicyDto request)
    {
        var policy = await _context.AccessPolicies
            .FirstOrDefaultAsync(p => p.Id == request.PolicyId);

        if (policy == null)
            return NotFound("Policy not found.");

        long direction = request.IsUp ? -1 : 1;
        long targetOrder = policy.Order + direction;

        var adjacentPolicy = await _context.AccessPolicies
            .FirstOrDefaultAsync(p => p.Order == targetOrder);

        if (adjacentPolicy == null)
            return BadRequest("Cannot move policy in the requested direction.");

        // Temporary order to avoid unique constraint collision
        const int TEMP_ORDER = -9999;

        adjacentPolicy.Order = TEMP_ORDER;
        await _context.SaveChangesAsync();

        adjacentPolicy.Order = policy.Order;
        policy.Order = targetOrder;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _context.AccessPolicies.FindAsync(id);
        if (entity == null) return NotFound();

        _context.AccessPolicies.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task HandleProperOrder(AccessPolicy entity)
    {
        bool isOrderAlreadyUsed = await _context.AccessPolicies.AnyAsync(p => p.Id != entity.Id && p.Order == entity.Order);
        
        if (entity.Order == 0 || isOrderAlreadyUsed)
        {
            // Find the current highest order value
            long lastOrder = await _context.AccessPolicies
                .OrderByDescending(e => e.Order)
                .Select(e => e.Order)
                .FirstOrDefaultAsync();

            entity.Order = lastOrder + 1;
        }
    }
    
}
