namespace SmoothNotesAPI.Models;
public class Profile
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PW { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string PrK { get; set; } = string.Empty;
    public string PuK { get; set; } = string.Empty;
    public DateTime CrDate { get; set; }
    public DateTime EdDate { get; set; }

    public Folder folders { get; set; }
}
