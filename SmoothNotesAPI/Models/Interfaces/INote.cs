namespace SmoothNotesAPI.Models.Interfaces;

public interface INote : IBase
{
    public Guid FolderId { get; set; }
    public string Text { get; set; }
    public string ESK { get; set; }
}
