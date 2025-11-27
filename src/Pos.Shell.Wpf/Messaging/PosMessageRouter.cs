using System.Collections.Concurrent;
using System.Text.Json;

using Microsoft.Web.WebView2.Wpf;

namespace Pos.Shell.Wpf.Messaging;

// 타입 세이프 메시지 라우터
public sealed class PosMessageRouter
{
    private readonly WebView2 _webView;

    // type → handler
    private readonly ConcurrentDictionary<string, Func<PosEnvelope, Task>> _handlers = new();

    public JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public PosMessageRouter(WebView2 webView)
    {
        _webView = webView;
    }

    /// <summary>
    /// TPayload 타입으로 Payload를 역직렬화해서 넘겨주는 핸들러 등록
    /// </summary>
    public void RegisterHandler<TPayload>(
        string type,
        Func<PosEnvelope, TPayload?, Task> handler)
    {
        _handlers[type] = async envelope =>
        {
            TPayload? payload = default;

            try
            {
                if (envelope.Payload.ValueKind != JsonValueKind.Undefined &&
                    envelope.Payload.ValueKind != JsonValueKind.Null)
                {
                    payload = envelope.Payload.Deserialize<TPayload>(JsonOptions);
                }
            }
            catch (JsonException ex)
            {
                // 역직렬화 실패는 프로토콜 오류로 간주
                await SendErrorAsync(
                    type: "protocol.error",
                    errorCode: "PAYLOAD_DESERIALIZE_FAILED",
                    errorMessage: ex.Message,
                    correlationId: envelope.CorrelationId);
                return;
            }

            await handler(envelope, payload);
        };
    }

    /// <summary>
    /// JS → WPF로부터 들어온 JSON을 처리
    /// </summary>
    public async Task HandleIncomingAsync(string json)
    {
        PosEnvelope? envelope;
        try
        {
            envelope = JsonSerializer.Deserialize<PosEnvelope>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            await SendErrorAsync(
                type: "protocol.error",
                errorCode: "INVALID_JSON",
                errorMessage: ex.Message);
            return;
        }

        if (envelope is null || string.IsNullOrWhiteSpace(envelope.Type))
        {
            await SendErrorAsync(
                type: "protocol.error",
                errorCode: "MISSING_TYPE",
                errorMessage: "Message Type is missing.");
            return;
        }

        if (_handlers.TryGetValue(envelope.Type, out var handler))
        {
            await handler(envelope);
        }
        else
        {
            // 등록되지 않은 타입은 무시 + 로깅 용도로 에러 전송
            await SendErrorAsync(
                type: "protocol.error",
                errorCode: "UNKNOWN_TYPE",
                errorMessage: $"No handler for '{envelope.Type}'.",
                correlationId: envelope.CorrelationId);
        }
    }

    /// <summary>
    /// Shell → SPA 로 메시지 전송
    /// </summary>
    public Task SendAsync<TPayload>(
        string type,
        TPayload? payload = default,
        bool isResponse = false,
        bool success = true,
        string? errorCode = null,
        string? errorMessage = null,
        string source = "shell",
        string target = "spa",
        string? correlationId = null)
    {
        var envelope = new PosEnvelope
        {
            Type = type,
            Source = source,
            Target = target,
            CorrelationId = correlationId,
            IsResponse = isResponse,
            Success = success,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage,
            CreatedAt = DateTimeOffset.UtcNow,
            // Payload는 JsonElement로 직접 채울 수 없으니 일단 TPayload를 직렬화 → 다시 JsonElement
            Payload = payload is null
                ? JsonDocument.Parse("null").RootElement
                : JsonSerializer.SerializeToElement(payload, JsonOptions)
        };

        var json = JsonSerializer.Serialize(envelope, JsonOptions);
        _webView.CoreWebView2.PostWebMessageAsJson(json);

        return Task.CompletedTask;
    }

    public Task SendErrorAsync(
        string type,
        string errorCode,
        string errorMessage,
        string? correlationId = null)
    {
        return SendAsync<object?>(
            type: type,
            payload: null,
            isResponse: true,
            success: false,
            errorCode: errorCode,
            errorMessage: errorMessage,
            correlationId: correlationId);
    }
}
