namespace SmoothNotesAPI.Models.Interfaces;

public interface IProfile : IBase
{
    public string PW { get; set; }
    public string Salt { get; set; }
    public string PrK { get; set; }
    public string PuK { get; set; }
    public List<Folder> folders { get; set; }
}
