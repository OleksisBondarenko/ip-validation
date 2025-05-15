
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADValidation.Data;
using ADValidation.DTOs.AccessPolicy;
using ADValidation.Mappers.AccessRule;
using ADValidation.Models.Access;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace ADValidation.Controllers;


[ApiController]
[Authorize]
[Route("api/[controller]")]
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
    public async Task<ActionResult<AccessPolicyDto>> Create(AccessPolicyDto createDto)
    {
        var entity = _mapper.MapFromCreateDto(createDto);

        _context.AccessPolicies.Add(entity);
        await _context.SaveChangesAsync();

        var dto = _mapper.MapToDto(entity);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, AccessPolicyDto updateDto)
    {
        var entity = await _context.AccessPolicies.FindAsync(id);
        if (entity == null) return NotFound();

        entity.Name = updateDto.Name;
        entity.Description = updateDto.Description;
        entity.IsActive = updateDto.IsActive;
        entity.IpFilterRules = updateDto.IpFilterRules;
        entity.Action = updateDto.Action;
        entity.Resource = updateDto.Resource;
        entity.ValidationTypes = updateDto.Validators;

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
    
}
