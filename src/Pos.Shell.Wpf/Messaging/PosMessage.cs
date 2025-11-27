namespace Pos.Shell.Wpf.Messaging;

public sealed class PosMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");   // 요청 식별자
    public string Type { get; set; } = string.Empty;                 // ping, sale.start 등
    public object? Payload { get; set; }                             // 임의 데이터
        = null;
    public bool IsResponse { get; set; }                             // 응답 여부
        = false;
    public bool Success { get; set; } = true;                        // 에러 플래그
    public string? Error { get; set; }                               // 에러 메시지
        = null;
}
