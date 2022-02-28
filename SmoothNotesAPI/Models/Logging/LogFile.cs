namespace SmoothNotesAPI.Models.Logging;
public class LogFile
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public Guid DeviceId { get; set; }
    public Guid ActionId { get; set; }
    public DateTime TimeStamp { get; set; }
}
