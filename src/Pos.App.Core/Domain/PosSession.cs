namespace Pos.App.Core.Domain;

public class PosSession
{
    public string StoreId { get; init; } = string.Empty;
    public string PosId { get; init; } = string.Empty;
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;
}
