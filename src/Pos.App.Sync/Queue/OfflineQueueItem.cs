namespace Pos.App.Sync.Queue;

public class OfflineQueueItem
{
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public bool Sent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
