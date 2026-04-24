using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskNotes.Data;
using RiskNotes.Models;
using System.Security.Claims;

namespace RiskNotes.Controllers;

[Authorize]
public class RiskNotesController : Controller
{
    private readonly AppDbContext _context;

    public RiskNotesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? sortBy)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var query = _context.RiskNotes
            .Where(n => n.UserId == userId)
            .AsQueryable();

        if(!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(n => 
                n.Title.Contains(searchTerm) ||
                n.Category.Contains(searchTerm) ||
                n.Severity.Contains(searchTerm) ||
                n.Description.Contains(searchTerm));
        }

        query = sortBy switch
        {
            "date_oldest" => query.OrderBy(n => n.CreatedAt),
            "severity" => query.OrderByDescending(n => 
                n.Severity == "Critical" ? 4:
                n.Severity == "High" ? 3:
                n.Severity == "Medium" ? 2 :
                n.Severity == "Low" ? 1 : 0),
                _ => query.OrderByDescending(n => n.CreatedAt)            
        };      

        ViewData["SearchTerm"] = searchTerm;
        ViewData["SortBy"] = sortBy;

        var notes = await query.ToListAsync();
        
        return View(notes);

        // var notes = new List<RiskNote>
        // {
        //     new RiskNote
        //     {
        //         Id = 1,
        //         Title = "VaR Model Review",
        //         Category = "Market Risk",
        //         Description = "Review assumptions"
        //     },
        //     new RiskNote
        //     {
        //         Id = 2,
        //         Title = "Stress Scenario",
        //         Category = "Stress Testing",
        //         Description = "Document adverse interest rate"
        //     }
        // };

        // return View(notes);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(RiskNote note)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if(!ModelState.IsValid)
        {
            return View(note);
        }

        note.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        note.CreatedAt = DateTime.Now;
        _context.RiskNotes.Add(note);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var note = await _context.RiskNotes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (note == null)
        {
            return NotFound();
        }

        return View(note);
    }

    [HttpGet]
    public async Task <IActionResult> Edit(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var note = await _context.RiskNotes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if(note == null)
        {
            return NotFound();
        }

        return View(note);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(RiskNote note)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if(!ModelState.IsValid)
        {
            return View(note);
        }

        note.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _context.RiskNotes.Update(note);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var note = await _context.RiskNotes.FindAsync(id);
        if(note == null)
        {
            return NotFound();
        }

        note.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return View(note);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var note = await _context.RiskNotes.FindAsync(id);
        if(note == null)
        {
            return NotFound();
        }
        note.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _context.RiskNotes.Remove(note);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Dashboard()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var notes = await _context.RiskNotes.ToListAsync();

        ViewData["TotalNotes"] = notes.Count(n => n.UserId == userId);
        ViewData["CriticalCount"] = notes.Count(n => n.Severity == "Critical" && n.UserId == userId);
        ViewData["HighCount"] = notes.Count(n => n.Severity == "High" && n.UserId == userId);
        ViewData["MediumCount"] = notes.Count(n => n.Severity == "Medium" && n.UserId == userId);
        ViewData["LowCount"] = notes.Count(n => n.Severity == "Low" && n.UserId == userId);

        return View();
    }
}


