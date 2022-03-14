namespace SmoothNotesAPI.Models.Login;

public class LProfile
{
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PuK { get; set; } = string.Empty;

    public List<Folder>? folders { get; set; }
}
