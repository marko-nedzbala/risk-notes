using System.ComponentModel.DataAnnotations;

namespace RiskNotes.Models;

public class RiskNote
{
    
    public int Id { get; set; }

    public string? UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = "";
    
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = "";
    
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    [StringLength(20)]
    public string Severity { get; set; } = "";

    public List<RiskNoteAttachment> Attachments { get; set; } = new();
}