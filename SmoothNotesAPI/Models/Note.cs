using SmoothNotesAPI.Models.Interfaces;

namespace SmoothNotesAPI.Models;
public class Note : INote
{
    public string Id { get; set; }
    public string FolderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CrDate { get; set; }
    public DateTime EdDate { get; set; }
}
