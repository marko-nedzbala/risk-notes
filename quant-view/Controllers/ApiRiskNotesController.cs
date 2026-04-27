using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskNotes.Data;
using RiskNotes.Models;
using System.Security.Claims;

namespace RiskNotes.Controllers;

[Authorize]
[Route("api/risknotes")]
[ApiController]
public class ApiRiskNotesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApiRiskNotesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<RiskNote>>> GetAll(int page = 1, int pageSize = 10)
    {
        if(page < 1)
        {
            page = 1;
        }

        if(pageSize < 1 || pageSize > 50)
        {
            pageSize = 10;
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var query = _context.RiskNotes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt);

        var totalCount = await query.CountAsync();

        var notes = await query
            .Skip( (page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
           Page = page,
           PageSize = pageSize,
           TotalCount = totalCount,
           Totalpages = (int)Math.Ceiling(totalCount / (double)pageSize),
           Data = notes
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RiskNote>> GetById(int id)
    {        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var note = await _context.RiskNotes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if(note == null)
        {
            return NotFound();
        }

        return Ok(note);
    }

    [HttpPost]
    public async Task<ActionResult<RiskNote>> Create(RiskNote note)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // note.Id = 0;
        note.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        note.CreatedAt = DateTime.Now;

        _context.RiskNotes.Add(note);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = note.Id },
            note
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, RiskNote updatedNote)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                  
        if (id != updatedNote.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingNote = await _context.RiskNotes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (existingNote == null)
        {
            return NotFound();
        }

        existingNote.Title = updatedNote.Title;
        existingNote.Category = updatedNote.Category;
        existingNote.Severity = updatedNote.Severity;
        existingNote.Description = updatedNote.Description;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var note = await _context.RiskNotes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (note == null)
        {
            return NotFound();
        }

        _context.RiskNotes.Remove(note);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}




