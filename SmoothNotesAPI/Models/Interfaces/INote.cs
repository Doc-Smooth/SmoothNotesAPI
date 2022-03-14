namespace SmoothNotesAPI.Models.Interfaces;

public interface INote : IBase
{
    public string FolderId { get; set; }
    public string Text { get; set; }
}
