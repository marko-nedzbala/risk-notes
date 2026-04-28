using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using RiskNotes.Data;
using RiskNotes.Models;
using System.Security.Claims;
using System.Text;
using ClosedXML.Excel;

namespace RiskNotes.Controllers;

[Authorize]
public class RiskNotesController : Controller
{
    private readonly AppDbContext _context;

    public RiskNotesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? sortBy, int page = 1)
    {
        int pageSize = 5;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var query = _context.RiskNotes
            .Where(n => n.UserId == userId)
            .Include(n => n.Attachments)
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

        var totalCount = await query.CountAsync();

        var notes = await query
            .Skip( (page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewData["CurrentPage"]  = page;
        ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize);
        
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
    public async Task<IActionResult> Create(RiskNote note, IFormFile? attachment)
    {          
        if(!ModelState.IsValid)
        {
            return View(note);
        }

        note.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        note.CreatedAt = DateTime.UtcNow;

        if(attachment != null && attachment.Length > 0)
        {
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads"
            );

            Directory.CreateDirectory(uploadsFolder);

            var storedFileName = $"{Guid.NewGuid()}_{Path.GetFileName(attachment.FileName)}";
            var filePath = Path.Combine(uploadsFolder, storedFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await attachment.CopyToAsync(stream);

            note.Attachments.Add(new RiskNoteAttachment
            {
                FileName = attachment.FileName,
                StoredFileName = storedFileName,
                ContentType = attachment.ContentType,
                FileSize = attachment.Length,
                UploadedAt = DateTime.UtcNow
            });
        }

        _context.RiskNotes.Add(note);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var note = await _context.RiskNotes
            .Include(n => n.Attachments)
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
        
        var notes = await _context.RiskNotes
            .Where(n => n.UserId == userId)
            .ToListAsync();

        // ViewData["TotalNotes"] = notes.Count(n => n.UserId == userId);
        // ViewData["CriticalCount"] = notes.Count(n => n.Severity == "Critical" && n.UserId == userId);
        // ViewData["HighCount"] = notes.Count(n => n.Severity == "High" && n.UserId == userId);
        // ViewData["MediumCount"] = notes.Count(n => n.Severity == "Medium" && n.UserId == userId);
        // ViewData["LowCount"] = notes.Count(n => n.Severity == "Low" && n.UserId == userId);

        ViewData["TotalNotes"] = notes.Count;
        ViewData["CriticalCount"] = notes.Count(n => n.Severity == "Critical");
        ViewData["HighCount"] = notes.Count(n => n.Severity == "High");
        ViewData["MediumCount"] = notes.Count(n => n.Severity == "Medium");
        ViewData["LowCount"] = notes.Count(n => n.Severity == "Low");

        return View();
    }

    public async Task<IActionResult> DownloadAttachement(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var attachment = await _context.RiskNoteAttachments
            .Include(a => a.RiskNote)
            .FirstOrDefaultAsync(a => a.Id == id && a.RiskNote.UserId == userId);

        if(attachment == null)
        {
            return NotFound();
        }

        var filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "uploads",
            attachment.StoredFileName
        );

        if(!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        return PhysicalFile(filePath, attachment.ContentType, attachment.FileName);
    }

    public async Task<IActionResult> ExportCsv()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var notes = await _context.RiskNotes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        var csv = new StringBuilder();

        csv.AppendLine("Id,Title,Category,Severity,Description,CreatedAt");

        foreach (var note in notes)
        {
            csv.AppendLine(
                $"\"{note.Id}\"," +
                $"\"{note.Title}\"," +
                $"\"{note.Category}\"," +
                $"\"{note.Severity}\"," +
                $"\"{note.Description}\"," +
                $"\"{note.CreatedAt}\""
            );
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());

        return File(bytes, "text/csv", "risk-notes.csv");
    }

    public async Task<IActionResult> ExportExcel()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var notes = await _context.RiskNotes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Risk Notes");

        worksheet.Cell(1, 1).Value = "Id";
        worksheet.Cell(1, 2).Value = "Title";
        worksheet.Cell(1, 3).Value = "Category";
        worksheet.Cell(1, 4).Value = "Severity";
        worksheet.Cell(1, 5).Value = "Description";
        worksheet.Cell(1, 6).Value = "Created At";

        var row = 2;

        foreach (var note in notes)
        {
            worksheet.Cell(row, 1).Value = note.Id;
            worksheet.Cell(row, 2).Value = note.Title;
            worksheet.Cell(row, 3).Value = note.Category;
            worksheet.Cell(row, 4).Value = note.Severity;
            worksheet.Cell(row, 5).Value = note.Description;
            worksheet.Cell(row, 6).Value = note.CreatedAt;

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var content = stream.ToArray();

        return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "risk-notes.xlsx"
        );
    }
}


