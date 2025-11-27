using System.Text.Json;

namespace Pos.Shell.Wpf.Messaging;

public interface IPosMessageRouter
{
    JsonSerializerOptions JsonOptions { get; }

    void RegisterHandler(string type, Func<PosMessage, Task> handler);

    Task HandleIncomingAsync(string json);

    Task SendAsync(string type, object? payload = null, bool isResponse = false,
        bool success = true, string? error = null);
}
