using SmoothNotesAPI.Models.Interfaces;

namespace SmoothNotesAPI.Models;
public class Folder : IFolder
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Fav { get; set; }
    public DateTime CrDate { get; set; }
    public DateTime EdDate { get; set; }

    public List<Note>? notes { get; set; }
}
