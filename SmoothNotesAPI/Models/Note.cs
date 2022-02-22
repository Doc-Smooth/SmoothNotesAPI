namespace SmoothNotesAPI.Models;
public class Note
{
    public Guid Id { get; set; }
    public Guid FolderId { get; set; }
    public Guid ProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string SK { get; set; } = string.Empty;
    public DateTime CrDate { get; set; }
    public DateTime EdDate { get; set; }
}
