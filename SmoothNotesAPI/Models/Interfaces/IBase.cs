namespace SmoothNotesAPI.Models.Interfaces;

public interface IBase
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CrDate { get; set; }
    public DateTime EdDate { get; set; }
}
