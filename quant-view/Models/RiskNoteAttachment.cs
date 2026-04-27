using System.ComponentModel.DataAnnotations;

namespace RiskNotes.Models;

public class RiskNoteAttachment
{
    public int Id { get; set; }
    
    public int RiskNoteId { get; set; }
    
    public RiskNote RiskNote { get; set; } = null!;

    [Required]
    public string FileName { get; set; } = "";
    
    [Required]
    public string StoredFileName { get; set; } = "";

    [Required]
    public string ContentType { get; set; } = "";

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.Now;
}





