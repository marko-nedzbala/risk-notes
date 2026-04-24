using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskNotes.Data;
using RiskNotes.Models;

namespace RiskNotes.Controllers;

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
    public async Task<ActionResult<List<RiskNote>>> GetAll()
    {
        var notes = await _context.RiskNotes
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

            return Ok(notes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RiskNote>> GetById(int id)
    {
        var note = await _context.RiskNotes.FindAsync(id);

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

        note.Id = 0;
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
        if (id != updatedNote.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingNote = await _context.RiskNotes.FindAsync(id);

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
        var note = await _context.RiskNotes.FindAsync(id);

        if (note == null)
        {
            return NotFound();
        }

        _context.RiskNotes.Remove(note);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}




