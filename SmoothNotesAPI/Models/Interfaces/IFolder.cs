namespace SmoothNotesAPI.Models.Interfaces;

public interface IFolder : IBase
{
    public string ProfileId { get; set; }
    public bool Fav { get; set; }
    public List<Note> notes { get; set; }
}
