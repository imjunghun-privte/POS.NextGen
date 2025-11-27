namespace Pos.App.Adapter.Printer;

public interface IReceiptPrinter
{
    Task PrintAsync(string text, CancellationToken ct = default);
}
