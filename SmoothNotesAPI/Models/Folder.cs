namespace SmoothNotesAPI.Models;
public class Folder
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Fav { get; set; }
    public DateTime CrDate { get; set; }
    public DateTime EdDate { get; set; }

    public Note notes { get; set; }
}
