using System.Collections.Concurrent;
using System.Text.Json;

using Microsoft.Web.WebView2.Wpf;

namespace Pos.Shell.Wpf.Messaging;

public sealed class PosMessageRouter
{
    private readonly WebView2 _webView;
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

    public async Task HandleIncomingAsync(string json)
    {
        PosEnvelope? envelope;

        try
        {
            envelope = JsonSerializer.Deserialize<PosEnvelope>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            await SendErrorAsync("protocol.error", "INVALID_JSON", ex.Message);
            return;
        }

        if (envelope is null || string.IsNullOrWhiteSpace(envelope.Type))
        {
            await SendErrorAsync("protocol.error", "MISSING_TYPE", "Message Type is missing.");
            return;
        }

        if (_handlers.TryGetValue(envelope.Type, out var handler))
        {
            await handler(envelope);
        }
        else
        {
            await SendErrorAsync(
                "protocol.error", "UNKNOWN_TYPE", $"No handler for '{envelope.Type}'.",
                correlationId: envelope.CorrelationId);
        }
    }

    public Task SendAsync<TPayload>(
        string type,
        TPayload? payload = default,
        bool isResponse = false,
        bool success = true,
        string? errorCode = null,
        string? errorMessage = null,
        string? correlationId = null)
    {
        var envelope = new PosEnvelope
        {
            Type = type,
            Source = "shell",
            Target = "spa",
            CorrelationId = correlationId,
            IsResponse = isResponse,
            Success = success,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage,
            CreatedAt = DateTimeOffset.UtcNow,
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
