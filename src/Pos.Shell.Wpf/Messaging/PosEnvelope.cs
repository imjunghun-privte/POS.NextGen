using System.Text.Json;

namespace Pos.Shell.Wpf.Messaging;

public sealed class PosEnvelope
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string Type { get; init; } = string.Empty;
    public string Source { get; init; } = "spa";
    public string Target { get; init; } = "shell";
    public string Version { get; init; } = "1.0";
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? CorrelationId { get; init; }

    public JsonElement Payload { get; init; }

    public bool IsResponse { get; init; }
    public bool Success { get; init; } = true;
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
}
