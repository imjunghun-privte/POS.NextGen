using Pos.App.Adapter.Printer;

namespace Pos.App.Adapter.Printer;

public class DebugReceiptPrinter : IReceiptPrinter
{
    public Task PrintAsync(string text, CancellationToken ct = default)
    {
        // 실제 매장에서는 장비 SDK 호출로 대체
        Console.WriteLine("[RECEIPT]");
        Console.WriteLine(text);
        Console.WriteLine("[/RECEIPT]");
        return Task.CompletedTask;
    }
}
