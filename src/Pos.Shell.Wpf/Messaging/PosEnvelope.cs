using System.Text.Json;

namespace Pos.Shell.Wpf.Messaging;

// 공통 메시지 Envelope (요청/응답 모두 사용)
public sealed class PosEnvelope
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string Type { get; init; } = string.Empty;          // ex) "ping", "product.list"
    public string Source { get; init; } = "spa";               // "spa" | "shell"
    public string Target { get; init; } = "shell";             // "spa" | "shell"
    public string Version { get; init; } = "1.0";              // 프로토콜 버전
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? CorrelationId { get; init; }                // 하나의 유스케이스 묶음 ID

    public JsonElement Payload { get; init; }                  // 실제 데이터(임의 구조)

    public bool IsResponse { get; init; }
    public bool Success { get; init; } = true;
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
}
